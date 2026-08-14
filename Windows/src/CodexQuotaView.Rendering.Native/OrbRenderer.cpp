#include "OrbRenderer.h"

#include <d3d11.h>
#include <d3dcompiler.h>
#include <wrl/client.h>

#include <cstring>

using Microsoft::WRL::ComPtr;

namespace {

const char kPixelShader[] =
    "struct PSInput { float4 position : SV_POSITION; float2 uv : TEXCOORD0; };\n"
    "cbuffer OrbConstants : register(b0) { float4 stateColor; float2 center; float radius; float timeSeconds; };\n"
    "float4 main(PSInput input) : SV_TARGET {\n"
    "  float2 offset = input.uv - center;\n"
    "  float distance = length(offset);\n"
    "  float edge = smoothstep(radius - 0.02, radius + 0.02, distance);\n"
    "  float wave = 0.5 + 0.5 * sin(offset.y * 24.0 - timeSeconds * 3.0 + length(offset) * 18.0);\n"
    "  float3 baseColor = stateColor.rgb * (0.55 + 0.45 * wave);\n"
    "  float alpha = 1.0 - edge;\n"
    "  return float4(baseColor * alpha, alpha);\n"
    "}\n";

const char kVertexShader[] =
    "struct VSInput { float2 position : POSITION; float2 uv : TEXCOORD0; };\n"
    "struct PSInput { float4 position : SV_POSITION; float2 uv : TEXCOORD0; };\n"
    "PSInput main(VSInput input) { PSInput output; output.position = float4(input.position, 0.0, 1.0); output.uv = input.uv; return output; }\n";

struct Vertex {
    float x;
    float y;
    float u;
    float v;
};

ComPtr<ID3D11Device> g_device;
ComPtr<ID3D11DeviceContext> g_context;
ComPtr<ID3D11PixelShader> g_pixelShader;
ComPtr<ID3D11VertexShader> g_vertexShader;
ComPtr<ID3D11Buffer> g_constantBuffer;
ComPtr<ID3D11Buffer> g_vertexBuffer;
ComPtr<ID3D11InputLayout> g_inputLayout;
ComPtr<ID3D11Texture2D> g_renderTarget;
ComPtr<ID3D11RenderTargetView> g_renderTargetView;
ComPtr<ID3D11Texture2D> g_staging;
int g_width = 0;
int g_height = 0;

bool EnsureDevice(int width, int height) {
    if (g_device && g_width == width && g_height == height) {
        return true;
    }

    g_device.Reset();
    g_context.Reset();
    g_pixelShader.Reset();
    g_vertexShader.Reset();
    g_constantBuffer.Reset();
    g_vertexBuffer.Reset();
    g_inputLayout.Reset();
    g_renderTarget.Reset();
    g_renderTargetView.Reset();
    g_staging.Reset();

    D3D_FEATURE_LEVEL featureLevels[] = {D3D_FEATURE_LEVEL_11_0, D3D_FEATURE_LEVEL_10_0};
    if (FAILED(D3D11CreateDevice(
            nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, 0, featureLevels, 2,
            D3D11_SDK_VERSION, g_device.GetAddressOf(), nullptr, g_context.GetAddressOf()))) {
        return false;
    }

    ComPtr<ID3DBlob> vertexBlob;
    ComPtr<ID3DBlob> pixelBlob;
    ComPtr<ID3DBlob> errorBlob;
    if (FAILED(D3DCompile(kVertexShader, strlen(kVertexShader), "vertex", nullptr, nullptr,
                          "main", "vs_5_0", 0, 0, vertexBlob.GetAddressOf(), errorBlob.GetAddressOf())) ||
        FAILED(D3DCompile(kPixelShader, strlen(kPixelShader), "pixel", nullptr, nullptr,
                          "main", "ps_5_0", 0, 0, pixelBlob.GetAddressOf(), errorBlob.GetAddressOf()))) {
        return false;
    }

    if (FAILED(g_device->CreateVertexShader(vertexBlob->GetBufferPointer(), vertexBlob->GetBufferSize(),
                                            nullptr, g_vertexShader.GetAddressOf())) ||
        FAILED(g_device->CreatePixelShader(pixelBlob->GetBufferPointer(), pixelBlob->GetBufferSize(),
                                           nullptr, g_pixelShader.GetAddressOf()))) {
        return false;
    }

    D3D11_INPUT_ELEMENT_DESC layoutDesc[] = {
        {"POSITION", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0},
        {"TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 8, D3D11_INPUT_PER_VERTEX_DATA, 0},
    };
    if (FAILED(g_device->CreateInputLayout(layoutDesc, 2, vertexBlob->GetBufferPointer(),
                                           vertexBlob->GetBufferSize(), g_inputLayout.GetAddressOf()))) {
        return false;
    }

    Vertex vertices[] = {
        {-1.0f, -1.0f, 0.0f, 1.0f},
        {1.0f, -1.0f, 1.0f, 1.0f},
        {-1.0f, 1.0f, 0.0f, 0.0f},
        {1.0f, 1.0f, 1.0f, 0.0f},
    };
    D3D11_BUFFER_DESC vertexDesc = {};
    vertexDesc.ByteWidth = sizeof(vertices);
    vertexDesc.Usage = D3D11_USAGE_IMMUTABLE;
    vertexDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    D3D11_SUBRESOURCE_DATA vertexData = {vertices, 0, 0};
    if (FAILED(g_device->CreateBuffer(&vertexDesc, &vertexData, g_vertexBuffer.GetAddressOf()))) {
        return false;
    }

    struct Constants {
        float color[4];
        float center[2];
        float radius;
        float timeSeconds;
    };
    D3D11_BUFFER_DESC constantDesc = {};
    constantDesc.ByteWidth = sizeof(Constants);
    constantDesc.Usage = D3D11_USAGE_DEFAULT;
    constantDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    if (FAILED(g_device->CreateBuffer(&constantDesc, nullptr, g_constantBuffer.GetAddressOf()))) {
        return false;
    }

    D3D11_TEXTURE2D_DESC textureDesc = {};
    textureDesc.Width = static_cast<UINT>(width);
    textureDesc.Height = static_cast<UINT>(height);
    textureDesc.MipLevels = 1;
    textureDesc.ArraySize = 1;
    textureDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
    textureDesc.SampleDesc.Count = 1;
    textureDesc.Usage = D3D11_USAGE_DEFAULT;
    textureDesc.BindFlags = D3D11_BIND_RENDER_TARGET;
    if (FAILED(g_device->CreateTexture2D(&textureDesc, nullptr, g_renderTarget.GetAddressOf())) ||
        FAILED(g_device->CreateRenderTargetView(g_renderTarget.Get(), nullptr, g_renderTargetView.GetAddressOf()))) {
        return false;
    }

    textureDesc.Usage = D3D11_USAGE_STAGING;
    textureDesc.BindFlags = 0;
    textureDesc.CPUAccessFlags = D3D11_CPU_ACCESS_READ;
    if (FAILED(g_device->CreateTexture2D(&textureDesc, nullptr, g_staging.GetAddressOf()))) {
        return false;
    }

    g_width = width;
    g_height = height;
    return true;
}

}  // namespace

extern "C" int orb_render_frame(const OrbRenderFrame* frame, uint8_t* rgbaOut) {
    if (!frame || !rgbaOut || frame->width <= 0 || frame->height <= 0 || !EnsureDevice(frame->width, frame->height)) {
        return 0;
    }

    float color[4] = {0.0f, 1.0f, 0.13f, 1.0f};
    switch (frame->state) {
        case 1:
            color[0] = 1.0f;
            color[1] = 0.8f;
            color[2] = 0.0f;
            break;
        case 2:
            color[0] = 1.0f;
            color[1] = 0.27f;
            color[2] = 0.23f;
            break;
        case 3:
            color[0] = 0.55f;
            color[1] = 0.55f;
            color[2] = 0.58f;
            break;
        default:
            break;
    }

    struct Constants {
        float color[4];
        float center[2];
        float radius;
        float timeSeconds;
    } constants;
    constants.color[0] = color[0];
    constants.color[1] = color[1];
    constants.color[2] = color[2];
    constants.color[3] = 1.0f;
    constants.center[0] = 0.5f;
    constants.center[1] = 0.5f;
    constants.radius = 0.38f;
    constants.timeSeconds = frame->timeSeconds;

    g_context->OMSetRenderTargets(1, g_renderTargetView.GetAddressOf(), nullptr);
    const float clearColor[4] = {0.0f, 0.0f, 0.0f, 0.0f};
    g_context->ClearRenderTargetView(g_renderTargetView.Get(), clearColor);
    g_context->IASetInputLayout(g_inputLayout.Get());
    UINT stride = sizeof(Vertex);
    UINT offset = 0;
    g_context->IASetVertexBuffers(0, 1, g_vertexBuffer.GetAddressOf(), &stride, &offset);
    g_context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP);
    g_context->VSSetShader(g_vertexShader.Get(), nullptr, 0);
    g_context->PSSetShader(g_pixelShader.Get(), nullptr, 0);
    g_context->UpdateSubresource(g_constantBuffer.Get(), 0, nullptr, &constants, 0, 0);
    g_context->PSSetConstantBuffers(0, 1, g_constantBuffer.GetAddressOf());
    g_context->Draw(4, 0);

    D3D11_BOX sourceBox = {0, 0, 0, static_cast<UINT>(frame->width), static_cast<UINT>(frame->height), 1};
    g_context->CopySubresourceRegion(g_staging.Get(), 0, 0, 0, 0, g_renderTarget.Get(), 0, &sourceBox);

    D3D11_MAPPED_SUBRESOURCE mapped = {};
    if (FAILED(g_context->Map(g_staging.Get(), 0, D3D11_MAP_READ, 0, &mapped))) {
        return 0;
    }
    const auto* source = static_cast<const uint8_t*>(mapped.pData);
    for (int row = 0; row < frame->height; ++row) {
        std::memcpy(rgbaOut + row * frame->width * 4, source + row * mapped.RowPitch, frame->width * 4);
    }
    g_context->Unmap(g_staging.Get(), 0);
    return 1;
}
