using PanPizza.Command;
using PanPizza.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PanPizza.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
		#region Fields
		readonly MainWindow mainView;
		private Array sizes = Enum.GetValues(typeof(Sizes));
        private string size;
        private List<Topping> toppingsArr;
        private Topping selectedTopping;
        private List<Toppings> toppingsToAdd;
        private double totalAmount;
        bool isOrdered;
        #endregion

        #region Constructor
        internal MainWindowViewModel(MainWindow view)
		{
			this.mainView = view;
            toppingsToAdd = new List<Toppings>();
            Size = string.Empty;
            toppingsArr = LoadToppings(); 
        }

        private List<Topping> LoadToppings()
        {
            var toppings = Enum.GetNames(typeof(Toppings));
            var toppingsList = new List<Topping>();
            for (int i = 0; i < toppings.Length; i++)
            {
                toppingsList.Add(new Model.Topping() { Name = toppings[i] });
            }
            return toppingsList;
        }
        #endregion

        #region Properties
        public double TotalAmount
        {
            get
            {
                return totalAmount;
            }
            set
            {
                totalAmount = value;
                OnPropertyChanged(nameof(TotalAmount));
            }
        }
        public Topping SelectedTopping
        {
            get
            {
                return selectedTopping;
            }
            set
            {
                if (selectedTopping == value) return;
                selectedTopping = value;
                OnPropertyChanged(nameof(SelectedTopping));
            }
        }
        public List<Topping> ToppingsArr
        {
            get
            {
                return toppingsArr;
            }
            set
            {
                if (toppingsArr == value) return;
                toppingsArr = value;
                OnPropertyChanged(nameof(ToppingsArr));
            }
        }
        public Array Sizes
        {
            get
            {
                return sizes;
            }
            set
            {
                if (sizes == value) return;
                sizes = value;
                OnPropertyChanged(nameof(Sizes));
            }
        }
        public string Size
        {
            get
            {
                return size;
            }
            set
            {
                if (size == value) return;
                size = value;
                OnPropertyChanged(nameof(Size));
            }
        }
        #endregion

        #region Methods

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        #endregion

        #region Commands
        //increasing the number of meals
        private ICommand addOneItem;
        public ICommand AddOneItem
        {
            get
            {
                if (addOneItem == null)
                {
                    addOneItem = new RelayCommand(param => AddOneItemExecute(), param => CanAddOneItem());
                }
                return addOneItem;
            }
        }

        private bool CanAddOneItem()
        {
            if (SelectedTopping == null
                || string.IsNullOrWhiteSpace(SelectedTopping.Name)
                || toppingsToAdd.Contains(ParseEnum<Toppings>(SelectedTopping.Name)))
                return false;
            return true;
        }

        private void AddOneItemExecute()
        {
            try
            {
                if (SelectedTopping != null)
                {
                    toppingsToAdd.Add(ParseEnum<Toppings>(SelectedTopping.Name));
                }
                else
                {
                    MessageBox.Show("[ERROR]");
                }
            }            
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //reduce the number of meals

        private ICommand removeOneItem;
        public ICommand RemoveOneItem
        {
            get
            {
                if (removeOneItem == null)
                {
                    removeOneItem = new RelayCommand(param => RemoveOneItemExecute(), param => CanRemoveOneItem());
                }
                return removeOneItem;
            }
        }

        private bool CanRemoveOneItem()
        {
            if (SelectedTopping == null
                || string.IsNullOrWhiteSpace(SelectedTopping.Name)
                || !toppingsToAdd.Contains(ParseEnum<Toppings>(SelectedTopping.Name))
                || isOrdered == true)
                return false;
            return true;
        }

        private void RemoveOneItemExecute()
        {
            try
            {
                if (SelectedTopping != null)
                {
                    toppingsToAdd.Remove(ParseEnum<Toppings>(SelectedTopping.Name));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //submiting the order

        private ICommand submitOrder;
        public ICommand SubmitOrder
        {
            get
            {
                if (submitOrder == null)
                {
                    submitOrder = new RelayCommand(param => SubmitOrderExecute(), param => CanSubmitOrderEmployee());
                }
                return submitOrder;
            }
        }

        private void SubmitOrderExecute()
        {
            try
            {
                var newPizza = new PizzaPan(ParseEnum<Sizes>(Size), toppingsToAdd);
                MessageBox.Show("Your order is successfully created!");
                TotalAmount = newPizza.GetPrice();
                isOrdered = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool CanSubmitOrderEmployee()
        {
            if (string.IsNullOrWhiteSpace(Size)
                || isOrdered == true)
                return false;
            return true;
        }
        #endregion
    }
}
