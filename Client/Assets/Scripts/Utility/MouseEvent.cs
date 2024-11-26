using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEvent : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
			PointerEventData pointerData = new PointerEventData(EventSystem.current)
			{
				position = Input.mousePosition
			};

			var raycastResults = new List<RaycastResult>();	
			EventSystem.current.RaycastAll(pointerData, raycastResults);

			// 如果有检测到元素
			if (raycastResults.Count > 0)
			{
				GameObject go = raycastResults[0].gameObject.transform.parent.parent.gameObject;
				int layer = go.layer;
				if(layer == LayerMask.NameToLayer("ChestPanel"))
				{
					layer += go.GetComponent<ChestPanel>().Offset;
				}
				EventHandler.CallMouseEvent(layer);
			}
		}
    }
}