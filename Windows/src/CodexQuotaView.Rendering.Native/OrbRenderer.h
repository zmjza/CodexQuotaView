#pragma once

#include <cstdint>

#ifdef __cplusplus
extern "C" {
#endif

struct OrbRenderFrame {
    int width;
    int height;
    int state;      // 0 normal, 1 warning, 2 exhausted, 3 idle
    float timeSeconds;
};

__declspec(dllexport) int orb_render_frame(const OrbRenderFrame* frame, uint8_t* rgbaOut);

#ifdef __cplusplus
}
#endif
