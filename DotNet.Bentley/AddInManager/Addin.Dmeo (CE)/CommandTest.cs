using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.MstnPlatformNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Addin.Dmeo
{
    public class CommandTest : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            var num = SelectionSetManager.NumSelected();

            var currentSet = new List<Element>();

            for (uint i = 0; i < num; i++)
            {
                Element elem = null;
                DgnModelRef dngModel = null;

                SelectionSetManager.GetElement(i, ref elem, ref dngModel);

                if (elem != null)
                {
                    currentSet.Add(elem);
                }
            }

            if (currentSet.Count == 0)
            {
                MessageBox.Show("当前未选择任何元素....");
            }
            else
            {
                MessageBox.Show(string.Format("当前已选择 {0} 个元素...", currentSet.Count));
            }
        }
    }
}
