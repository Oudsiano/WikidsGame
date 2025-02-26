using System.Collections.Generic;
using DialogueEditor;
using UI;
using UI.Inventory;
using UI.Inventory.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace AINavigation
{
    public class CheckItem : MonoBehaviour // TODO rename
    {
        [FormerlySerializedAs("nameParamInNPC")] [SerializeField]
        private string _nameParamInNPC;

        [FormerlySerializedAs("items")] [SerializeField]
        private List<ItemDefinition> _items = new();

        [FormerlySerializedAs("nameItem")]
        [Header("�������� ��������� (�� ��� ���� �����������)")] // TODO UTF-8 error
        [SerializeField]
        private string _nameItem;

        [FormerlySerializedAs("itemsString")] [SerializeField]
        private List<string> _itemsString = new();

        private UIManager _uiManager;
        
        public void Construct(UIManager uiManager)
        {
            _uiManager = uiManager;
        }
        
        public void CheсkItem() // TODO not used // TODO rename
        {
            bool confirm = true;

            foreach (var item in _items)
            {
                if (_uiManager.uIBug.TryTakeQuestItem(item.Name) == false)
                {
                    confirm = false;
                }
            }

            foreach (var item in _itemsString)
            {
                if (_uiManager.uIBug.TryTakeQuestItem(item) == false)
                {
                    confirm = false;
                }
            }

            if (_uiManager.uIBug.TryTakeQuestItem(_nameItem) == false)
            {
                confirm = false;
            }

            ConversationManager.Instance.SetBool(_nameParamInNPC, confirm);
        }

        public void CheckItemAndDelete()
        {
            bool confirm = true;

            foreach (var item in _items)
            {
                if (_uiManager.uIBug.TryTakeQuestItem(item.Name) == false)
                    confirm = false;
            }

            foreach (var item in _itemsString)
            {
                if (_uiManager.uIBug.TryTakeQuestItem(item) == false)
                    confirm = false;
            }

            if (_uiManager.uIBug.TryTakeQuestItem(_nameItem) == false)
            {
                confirm = false;
            }

            if (confirm)
            {
                foreach (var item in _items)
                {
                    _uiManager.uIBug.NeedDeleteItem(item.Name);
                }

                foreach (var item in _itemsString)
                {
                    _uiManager.uIBug.NeedDeleteItem(item);
                }

                _uiManager.uIBug.NeedDeleteItem(_nameItem);
            }

            ConversationManager.Instance.SetBool(_nameParamInNPC, confirm);
        }
    }
}