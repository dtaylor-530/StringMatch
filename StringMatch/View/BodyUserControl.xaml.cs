using System;
using System.Collections.Generic;
using System.Data;
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

namespace StringMatch
{
   
    /// <summary>
    /// Interaction logic for BodyUserControl.xaml
    /// </summary>
    public partial class BodyUserControl : UserControl
    {
        public BodyUserControl()
        {
            InitializeComponent();
        }



        private DataRowView rowBeingEdited = null;

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataRowView rowView = e.Row.Item as DataRowView;
            rowBeingEdited = rowView;
        }

        private void dataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (rowBeingEdited != null)
            {
                rowBeingEdited.EndEdit();
            }
        }


        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            DependencyObject dep = (DependencyObject)e.OriginalSource;


            var p = DataGridHelpers.GetTableIndices(dep);
            if (p == null) return;

            if (p.Value.Y != 1)
            {
                try
                {
                    string content = (dgPotential.GetCell(p.Value.X,1).Content as TextBlock).Text;
                    string content2 = (dgPotential.GetCell(p.Value.X, p.Value.Y).Content as TextBlock).Text;

                    (dgPotential.GetCell(p.Value.X, p.Value.Y).Content as TextBlock).SetValue(TextBlock.TextProperty, content);
                    (dgPotential.GetCell(p.Value.X, 1).Content as TextBlock).SetValue(TextBlock.TextProperty, content2);



                }
                catch { }
            }


                (dgPotential.GetCell(p.Value.X, 0).Content as CheckBox).IsChecked = true;

        }

        private void dgPotential_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            DependencyObject dep = (DependencyObject)e.OriginalSource;

             var p = DataGridHelpers.GetTableIndices(dep);
            if (p != null)
                if (p.Value.Y == 0 )
                {
                    (dgPotential.GetCell(p.Value.X, 0).Content as CheckBox).IsChecked = !(dgPotential.GetCell(p.Value.X, 0).Content as CheckBox).IsChecked;
                }




        }
    }
}
