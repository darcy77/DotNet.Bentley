using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Bentley.AddInManager.Model
{
    /// <summary>
    /// AddinAssemblyModel
    /// </summary>
    class AddinAssemblyModel
    {
        /// <summary>
        /// 路径
        /// </summary>

        public string Path { get; set; }

        /// <summary>
        /// 类型
        /// </summary>

        public List<string> Types { get; set; }

        public AddinAssemblyModel()
        {
            this.Types = new List<string>();
        }

        /// <summary>
        /// Converters the specified models.
        /// </summary>
        /// <param name="models">The models.</param>
        /// <returns></returns>
        public static List<AddinAssemblyModel> Converter(IEnumerable<AddInModel> models)
        {
            var result = new List<AddinAssemblyModel>();

            using (var eum = models.GetEnumerator())
            {
                while (eum.MoveNext())
                {
                    var model = new AddinAssemblyModel() { Path = eum.Current.Path };

                    if (eum.Current.Childs != null)
                    {
                        foreach (var item in eum.Current.Childs)
                        {
                            model.Types.Add(item.Name);
                        }
                    }

                    if (model.Types.Count > 0)
                    {
                        result.Add(model);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Converters the specified models.
        /// </summary>
        /// <param name="models">The models.</param>
        /// <returns></returns>
        public static ObservableCollection<AddInModel> Converter(IList<AddinAssemblyModel> models)
        {
            var result = new ObservableCollection<AddInModel>();

            using (var eum = models.GetEnumerator())
            {
                while (eum.MoveNext())
                {
                    if (File.Exists(eum.Current.Path))
                    {
                        var model = new AddInModel() { Path = eum.Current.Path, Name = System.IO.Path.GetFileNameWithoutExtension(eum.Current.Path) };

                        if (eum.Current.Types != null)
                        {
                            foreach (var item in eum.Current.Types)
                            {
                                model.Childs.Add(new AddInModel()
                                {
                                    Name = item,
                                    Parent = model,
                                    Path = eum.Current.Path
                                });
                            }
                        }

                        result.Add(model);
                    }
                }
            }
            return result;
        }
    }
}
