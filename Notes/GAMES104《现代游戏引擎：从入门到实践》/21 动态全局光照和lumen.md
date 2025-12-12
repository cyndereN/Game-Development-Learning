# 21 动态全局光照和Lumen

## GI

montecarlo integration

Sampling is the key 采样越多越清晰，性能消耗越大

Importance sampling: PDf is better

## RSM

以光源为起点。

## LPV

Use a 3D grid to propagate from directly illuminated surfaces to anywhere else 

VXGI 都用了 RSM

## SSGI

Radiance sampling in screen space: Reuse screen-space data

Linear remarching, ray reusing among neighbor pixels

Cone tracing with mipmap filtering

## Lumen

Why?

- Ray traces are slow. Can only afford 1/2 ray per pixel. But quality GI requires hundreds.

- Sampling is hard.

How?

Phase 1: 在任何硬件上进行快速的RT（Ray Trace）

SDF: singed distance field. Per-mesh SDF

Phase 2: 光线注入和缓存

每个物体需要采样六个面得到直接光照，然后在以此为基础进行间接光照计算，相当于每个采样面都是一个probe。
