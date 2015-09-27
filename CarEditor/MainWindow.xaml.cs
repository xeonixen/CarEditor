using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarGameEngine;
using System.ComponentModel;

namespace CarEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public class CarEvent : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Car car = new Car();
        public string Name
        {
            get
            {
                return car.Name;
            }
            set
            {
                car.Name = value;
            }
        }
        public Dictionary<Int32,Int32> Torque
        {
            get
            {
                return car.engine._torqueCurve;
            }
            set
            {
                car.engine._torqueCurve = value;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {


            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }


    public partial class MainWindow : Window
    {
        public List<Label> torqueLabelList = new List<Label>();
        public List<Label> torqueRpmLabelList = new List<Label>();
        public List<Slider> torqueSliderList = new List<Slider>();
        public CarEvent carEvent = new CarEvent();

        public MainWindow()
        {
            try
            {
                carEvent.car = CarTools.LoadCarZip("car.dat");
            }
            catch (Exception)
            {
                carEvent.car = new Car();
            }
            
            InitializeComponent();
            CarNameText.SetBinding(TextBox.TextProperty, new Binding("Name")
            {
                Source = carEvent,
                Mode = BindingMode.TwoWay
            });
            int i = 0;
            foreach (KeyValuePair<Int32,Int32> t in carEvent.Torque)
            {
                Label temp1 = new Label();
                Label temp2 = new Label();
                Slider temp3 = new Slider();
                temp3.ValueChanged += onSliderChange;
                temp1.Content = t.Key.ToString();
                temp2.Content = t.Value.ToString();
                temp3.Value = t.Value;
                temp3.Maximum = 3000;
                temp3.Minimum = 0;
                temp1.Margin = new Thickness(20, 80+i*16, 0, 0);
                temp2.Margin = new Thickness(90, 80 + i * 16, 0, 0);
                temp3.Margin = new Thickness(160, 80 + i * 16, 10, 0);
                
                i++;
                rootGrid.Children.Add(temp1);
                torqueRpmLabelList.Add(temp1);
                rootGrid.Children.Add(temp2);
                torqueLabelList.Add(temp2);
                rootGrid.Children.Add(temp3);
                torqueSliderList.Add(temp3);

            }


        }


    public void onSliderChange(object sender, EventArgs e)
        {
            if (torqueSliderList.Contains((Slider)sender)){
                var index = torqueSliderList.IndexOf((Slider)sender);
                torqueLabelList[index].Content = (int)torqueSliderList[index].Value;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Dictionary<Int32, Int32> temp = new Dictionary<int, int>();
            for(int i=0;i<torqueRpmLabelList.Count;i++)
            {
                temp.Add(Convert.ToInt32(torqueRpmLabelList[i].Content), Convert.ToInt32(torqueLabelList[i].Content));
            }
            carEvent.Torque = temp;
            CarTools.SaveCarZip("car.dat", carEvent.car);
        }
    }
    
}
