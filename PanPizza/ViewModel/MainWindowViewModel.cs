using PanPizza.Command;
using PanPizza.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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
		string phone;
		SerialPort port = new SerialPort();
		clsSMS objclsSMS = new clsSMS();
		ShortMessageCollection objShortMessageCollection = new ShortMessageCollection();
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
			try
			{
				var toppings = Enum.GetNames(typeof(Toppings));
				var toppingsList = new List<Topping>();
				for (int i = 0; i < toppings.Length; i++)
				{
					toppingsList.Add(new Model.Topping() { Name = toppings[i] });
				}
				return toppingsList;
			}
			catch (Exception)
			{
				return new List<Topping>();
			}
		}
		#endregion

		#region Properties

		public string Phone
		{
			get
			{
				return phone;
			}
			set
			{
				if (phone == value) return;
				phone = value;
				OnPropertyChanged(nameof(Phone));
			}
		}
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
		//add topping
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
			try
			{
				if (SelectedTopping == null
				|| string.IsNullOrWhiteSpace(SelectedTopping.Name)
				|| toppingsToAdd.Contains(ParseEnum<Toppings>(SelectedTopping.Name)))
					return false;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void AddOneItemExecute()
		{
			try
			{
				if (SelectedTopping != null)
				{
					toppingsToAdd.Add(ParseEnum<Toppings>(SelectedTopping.Name));
					var addedTopping = ToppingsArr.FirstOrDefault(x => x.Name == selectedTopping.Name);
					ToppingsArr = UpdateToppings(addedTopping, true);
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
		private List<Topping> UpdateToppings(Topping addedTopping, bool v)
		{
			if (ToppingsArr.Contains(addedTopping))
			{
				ToppingsArr.First(x => x.Name == addedTopping.Name).IsAdded = v;
			}
			return ToppingsArr;
		}

		//remove topping

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
			try
			{
				if (SelectedTopping == null
				  || string.IsNullOrWhiteSpace(SelectedTopping.Name)
				  || !toppingsToAdd.Contains(ParseEnum<Toppings>(SelectedTopping.Name))
				  || isOrdered == true)
					return false;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void RemoveOneItemExecute()
		{
			try
			{
				if (SelectedTopping != null)
				{
					toppingsToAdd.Remove(ParseEnum<Toppings>(SelectedTopping.Name));
					var removedTopping = ToppingsArr.FirstOrDefault(x => x.Name == selectedTopping.Name);
					ToppingsArr = UpdateToppings(removedTopping, false);
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
					submitOrder = new RelayCommand(param => SubmitOrderExecute(), param => CanSubmitOrder());
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

		private bool CanSubmitOrder()
		{
			try
			{
				if (string.IsNullOrWhiteSpace(Size)
			   || isOrdered == true)
					return false;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		//Send SMS

		private ICommand sendSMS;
		public ICommand SendSMS
		{
			get
			{
				if (sendSMS == null)
				{
					sendSMS = new RelayCommand(param => SendSMSExecute(), param => CanSendSMS());
				}
				return sendSMS;
			}
		}

		private void SendSMSExecute()
		{
			try
			{
				if (objclsSMS.sendMsg(this.port, Phone, "Your pizza order iz created."))
				{
					MessageBox.Show("Message has sent successfully");

				}
				else
				{
					MessageBox.Show("Failed to send message");
				}
			}
			catch (Exception)
			{
				MessageBox.Show("Something went wrong. Message is not sent.");
			}
		}

		private bool CanSendSMS()
		{
			if (!isOrdered
				|| string.IsNullOrWhiteSpace(Phone))
				return false;
			return true;
		}
		#endregion
	}
}
