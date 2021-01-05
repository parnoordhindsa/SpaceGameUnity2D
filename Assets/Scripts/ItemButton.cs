using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text;

public class ItemButton : MonoBehaviour , IPointerEnterHandler , IPointerExitHandler
{
    public int buttonID;
    public Item thisItem;

    public ToolTip tooltip;
    private Vector2 position;

    private Item GetThisItem(){
        for (int i = 0; i < GameManager.instance.items.Count; i++){
            if (buttonID == i)
            {
                thisItem = GameManager.instance.items[i];
            }
        }
        return thisItem;
    }
    public void CloseButton(){
        GameManager.instance.RemoveItem(GetThisItem());

        thisItem = GetThisItem();
        if (thisItem != null){
            tooltip.ShowToolTip();
            tooltip.UpdateToolTip(GetDetailText(thisItem));
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.Find("Canvas").transform as RectTransform, Input.mousePosition, null, out position);
            tooltip.SetPosition(position);

        }else{
            tooltip.HideToolTip();
            tooltip.UpdateToolTip("");
        }
    }

    public void OnPointerEnter(PointerEventData eventData){
        GetThisItem();
        if (thisItem != null){
            // Debug.Log("Enter " + thisI tem.itemName + " slots");

            tooltip.ShowToolTip();
            tooltip.UpdateToolTip(GetDetailText(thisItem));
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.Find("Canvas").transform as RectTransform, Input.mousePosition, null, out position);
            tooltip.SetPosition(position);
        }
        
    }

    public void OnPointerExit(PointerEventData eventData){
        // if (thisItem != null){
            // Debug.Log("Exit " + thisItem.itemName + " slots");

            tooltip.HideToolTip();
            tooltip.UpdateToolTip("");
        // }
    }

    private string GetDetailText(Item _item){
        if (_item == null){
            return "";
        }
        else{
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Item: {0}\n\n", _item.itemName);
            stringBuilder.AppendFormat("Description: {0}\n\n",
                                        _item.itemDescription);

            return stringBuilder.ToString();
        }
    }
}
