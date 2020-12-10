using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace tftkeiba.Views
{
    /// <summary>
    /// Interaction logic for MenuItem
    /// </summary>
    public partial class MenuBar : Menu
    {

        /*   public static readonly DependencyProperty WhenColsedChildWindowCommandProperty = DependencyProperty.Register("WhenColsedChildWindowCommand", typeof(ICommand), typeof(MenuBar));
           public ICommand WhenColsedChildWindowCommand
           {
               get { return (ICommand)GetValue(WhenColsedChildWindowCommandProperty); }
               set { SetValue(WhenColsedChildWindowCommandProperty, value); }
           }*/

        public MenuBar()
        {
            InitializeComponent();
        }
    }
}
