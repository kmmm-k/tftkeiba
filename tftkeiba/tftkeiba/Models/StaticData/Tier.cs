using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba.Models.StaticData
{
    public class TierList : List<Tier>
    {
    }

    public class Tier : BindableBase
    {
        public Tier()
        {
        }
        private string _key;
        public string key
        {
            get
            {
                return _key;
            }
            set
            {
                SetProperty(ref _key, value);
            }
        }
        private bool? isChecked = false;
        public bool? IsChecked
        {
            set
            {
                if (value == true)
                {
                    foreach(var d in divisions ?? new ObservableCollection<Division>())
                    {
                        d.IsChecked = true;
                    }
                }
                else if (value == false)
                {
                    foreach (var d in divisions ?? new ObservableCollection<Division>())
                    {
                        d.IsChecked = false;
                    }
                }
                SetProperty(ref isChecked, value);
            }
            get
            {
                if (divisions == null || divisions.Count() == 0)
                {
                    return isChecked;
                }
                else
                {
                    var t = divisions.Any(q => q.IsChecked == true);
                    var f = divisions.Any(q => q.IsChecked == false);
                    if (t && f) return null;
                    else if (t) return true;
                    else if (f) return false;
                    else return null;
                }
            }
        }
        public string order { get; set; }
        public string api { get; set; }
        private ObservableCollection<Division> _divisions;
        public ObservableCollection<Division> divisions { 
            get
            {
                return _divisions;
            }
            set
            {
                SetProperty(ref _divisions, value);
                foreach(var d in value ?? new ObservableCollection<Division>())
                {
                    d.OnCheckChanged += (object sender, EventArgs e) =>
                    {
                        RaisePropertyChanged(nameof(IsChecked));
                    };
                }
            }
        }
        public bool IsSpecial { 
            get
            {
                if (divisions == null || divisions.Count() == 0) return true;
                return false;
            } 
        }
    }

    public class Division : BindableBase
    {
        public event EventHandler OnCheckChanged;

        private string _key;
        public string key
        {
            get
            {
                return _key;
            }
            set
            {
                SetProperty(ref _key, value);
            }
        }
        private bool? isChecked = false;
        public bool? IsChecked
        {
            set
            {
                SetProperty(ref isChecked, value);
                OnCheckChanged?.Invoke(this, new EventArgs());
            }
            get
            {
                return isChecked;
            }
        }
        public string order { get; set; }
        public string api { get; set; }
    }

}
