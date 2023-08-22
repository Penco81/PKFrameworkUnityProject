using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComponentOptimizing
{
    #region Image
    public static void OptimizingImage(Image image)
    {
        image.raycastTarget = false;
    }
    #endregion


    #region Text
    public static void OptimizingText(Text text)
    {
        text.raycastTarget = false;
        text.supportRichText = false;
    }
    #endregion
    
    #region Text
    public static void OptimizingRawImage(RawImage rawImage)
    {
        rawImage.raycastTarget = false;
    }
    #endregion

    #region Mask
    public static void OptimizingMask(Mask mask)
    {
        var canvas = mask.GetComponentInParent<Canvas>();
        if(canvas == null)
        {
            return;
        }
        // TODO UI上mask转Rect Mask 2D
    }
    #endregion
}