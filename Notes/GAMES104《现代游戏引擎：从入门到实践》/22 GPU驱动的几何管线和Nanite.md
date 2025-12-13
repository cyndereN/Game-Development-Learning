# 22 GPU驱动的几何管线和Nanite

## GPU Driven Pipeline in Assassins Creed

把越来越多的工作移交给GPU，并存储在显存，以减少CPU的工作和数据传输速度的瓶颈，大大提高效率。

Draw primitive -> draw scene

把三角形面片分成一个个Clusters，并以此完成各种渲染运算，提高几何利用率。

## Occlusion Culling for Camera and Shadow

用上一帧的Occlusion Depth做遮挡判断。

用上一帧提前做一次深度测试判断可见性，在用当前帧对上一帧过滤掉的不可见做一次测试，判断是否有新的可见。

用上一帧的一张低精度深度图剔除不需要进行阴影绘制的物体，减少需要生成阴影的几何，提高阴影生成速度。

##　Visibility Buffer

## Nanite Overview

只在缓存中存储view视角下基于LOD的纹理，把这种思想推衍到几何面片中。

## Nanite Geometry Representation