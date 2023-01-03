using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProperty : MonoBehaviour
{
    RectTransform _rt = null;
    protected RectTransform myRT
    {
        get
        {
            if (_rt == null)
            {
                _rt = GetComponent<RectTransform>();
            }
            return _rt;
        }
    }

    Image _img = null;
    protected Image myImage
    {
        get
        {
            if (_img == null)
            {
                _img = GetComponent<Image>();
            }
            return _img;
        }
    }
}
