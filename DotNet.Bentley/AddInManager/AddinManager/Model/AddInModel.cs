using DotNet.MVVM.Model;
using DotNet.MVVM;
using System.Collections.ObjectModel;

namespace Bentley.AddInManager.Model
{
    [Notifier]
    class AddInModel : ObservableObject
    {
        /// <summary>
        /// 父级
        /// </summary>
        public AddInModel Parent { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
 
        /// <summary>
        /// 程序集路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 展开
        /// </summary>
        public bool IsExpand { get; set; }

        /// <summary>
        /// 子节点.
        /// </summary>

        public ObservableCollection<AddInModel> Childs { get; set; }

        public AddInModel()
        {
            this.IsExpand = true;
            this.Childs = new ObservableCollection<AddInModel>();
        }
    }
}
