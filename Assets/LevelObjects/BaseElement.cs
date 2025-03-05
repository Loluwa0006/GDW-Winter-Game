using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

[System.Serializable]
public class BaseElement : MonoBehaviour
{

   public enum ElementType
    {
        AIR,
        WATER,
        EARTH,
        FIRE,
        GRAVITY
    }

   [SerializeField] ElementType elementType;

   public List<ElementType> reactableElements = new List<ElementType>();

  
    public ElementType GetElementType()
    {
        return elementType;
    }

    public List<ElementType> GetReactableElements()
    {
        return reactableElements;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        BaseElement collider_element = collision.gameObject.GetComponent<BaseElement>();
        if (collider_element == null) return;
        foreach (ElementType reactions in reactableElements)
        {
            if (collider_element.GetElementType() == reactions)
            {
                onElementalReaction();
            }
        }
    }

 

    public virtual void onElementalReaction()
    {

    }

}
