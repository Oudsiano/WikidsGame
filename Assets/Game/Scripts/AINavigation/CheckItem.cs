using System.Collections.Generic;
using DialogueEditor;
using FarrokhGames.Inventory.Examples;
using UnityEngine;

namespace AINavigation
{
    public class CheckItem : MonoBehaviour
    {
        [SerializeField] public string nameParamInNPC;
        [SerializeField] public List<ItemDefinition> items = new List<ItemDefinition>();

        [Header("�������� ��������� (�� ��� ���� �����������)")]

        [SerializeField] public string nameItem;
        [SerializeField] public List<string> itemsString = new List<string>();

        public void ChekItem()
        {
            bool confirm = true;

            foreach (var item in items)
            {
                if (!IGame.Instance.UIManager.uIBug.TryTakeQuestItem(item.Name))
                    confirm = false;
            }

            foreach (var item in itemsString)
            {
                if (!IGame.Instance.UIManager.uIBug.TryTakeQuestItem(item))
                    confirm = false;
            }
        
            if (!IGame.Instance.UIManager.uIBug.TryTakeQuestItem(nameItem))
            {
                confirm = false;
            }

            ConversationManager.Instance.SetBool(nameParamInNPC, confirm);
        }

        public void ChekItemAndDelte()
        {
            bool confirm = true;

            foreach (var item in items)
            {
                if (!IGame.Instance.UIManager.uIBug.TryTakeQuestItem(item.Name))
                    confirm = false;
            }

            foreach (var item in itemsString)
            {
                if (!IGame.Instance.UIManager.uIBug.TryTakeQuestItem(item))
                    confirm = false;
            }

            if (!IGame.Instance.UIManager.uIBug.TryTakeQuestItem(nameItem))
            {
                confirm = false;
            }

            if (confirm)
            {
                foreach (var item in items)
                    IGame.Instance.UIManager.uIBug.NeedDeleteItem(item.Name);
                foreach (var item in itemsString)
                    IGame.Instance.UIManager.uIBug.NeedDeleteItem(item);
                IGame.Instance.UIManager.uIBug.NeedDeleteItem(nameItem);
            }

            ConversationManager.Instance.SetBool(nameParamInNPC, confirm);
        }
    }
}
