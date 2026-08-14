// Visual reference: QuotaView stable Activity Island orb (macOS Metal).
// This HLSL pixel shader approximates the orb's state colors and motion for
// the Windows projection. It is not a port of Apple's Liquid Glass API.

struct PSInput {
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD0;
};

cbuffer OrbConstants : register(b0) {
    float4 stateColor;
    float2 center;
    float radius;
    float timeSeconds;
};

float4 main(PSInput input) : SV_TARGET {
    float2 offset = input.uv - center;
    float distance = length(offset);
    float edge = smoothstep(radius - 0.02, radius + 0.02, distance);
    float wave = 0.5 + 0.5 * sin(offset.y * 24.0 - timeSeconds * 3.0 + length(offset) * 18.0);
    float3 baseColor = stateColor.rgb * (0.55 + 0.45 * wave);
    float alpha = 1.0 - edge;
    return float4(baseColor * alpha, alpha);
}
