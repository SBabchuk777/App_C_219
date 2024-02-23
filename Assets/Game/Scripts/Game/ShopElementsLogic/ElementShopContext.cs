using System;
using System.Collections.Generic;
using System.Linq;
using CodeHub.OtherUtilities;
using Game.Mephistoss.PanelMachine.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Game.ShopLogic
{
    public class ElementShopContext : MonoBehaviour
    {
        [SerializeField] private PlayerDatabase _playerDatabase;
        
        [SerializeField] private ShopElementContextData _contextData;
        [SerializeField] private List<ElementShopBtn> _shopBtns;

        [SerializeField] private AudioSource _successBuy;
        [SerializeField] private AudioSource _notSuccessBuy;
        [SerializeField] private AudioSource _click;

        [SerializeField] private GameObject _playerBalanceToAnim;

        public UnityEvent _onElementSelected;

        private AnimationService _animationService;

        private ElementShopBtn _currentShopElementBtn;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            InitializeShopElements();
            UpdateUiShopElements();
            _animationService = new AnimationService();
        }

        private void ClickOnBuyShopElement(ElementShopBtn elementBtn)
        {
            _currentShopElementBtn = elementBtn;
            TryBuyElement(_currentShopElementBtn);

            _click.Play();
        }

        private void TryBuyElement(ElementShopBtn elementBtn)
        {
            int price = elementBtn.ElementData.Price;
            if (price <= _playerDatabase.PlayerBalance)
            {
                _playerDatabase.IncreasePlayerBalance(-price);
                elementBtn.ElementData.HasOpen = true;
                Select(elementBtn);

                _successBuy.Play();
            }
            else
            {
                _notSuccessBuy.Play();
                _animationService.StartShakeAnimation(_playerBalanceToAnim);
            }
        }

        private void Select(ElementShopBtn elementBtn)
        {
            _contextData.SetCurrentElement(elementBtn.ElementData);
            UpdateUiShopElements();

            _click.Play();
            _onElementSelected?.Invoke();
        }

        private void UpdateUiShopElements()
        {
            foreach (var shopBtn in _shopBtns)
            {
                shopBtn.UpdateUI();
            }

            TrySelectCurrentElement();
        }

        private void TrySelectCurrentElement()
        {
            var currentElement = _contextData.GetCurrentElement();
            currentElement.HasOpen = true;
            var currentElementsBtn = _shopBtns.FirstOrDefault(hook => hook.ElementData == currentElement);

            if (currentElementsBtn != null)
            {
                currentElementsBtn.Select();
                _onElementSelected?.Invoke();
            }
        }

        private void InitializeShopElements()
        {
            foreach (var shopBtn in _shopBtns)
            {
                shopBtn.onBuy += ClickOnBuyShopElement;
                shopBtn.onSelect += Select;
            }
        }
    }
}