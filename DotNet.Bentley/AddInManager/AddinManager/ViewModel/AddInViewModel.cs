using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.MVVM.Model;
using DotNet.MVVM;
using Bentley.AddInManager.Model;
using System.Collections.ObjectModel;
using AddInManager.Helper;
using System.Windows.Input;
using System.Reflection;
using Microsoft.Win32;
using Mono.Cecil;
using System.IO;
using AddinManager.Helper;

namespace AddInManager.ViewModel
{
    [Notifier]
    class AddInViewModel : ViewModelBase
    {
        public ObservableCollection<AddInModel> Models { get; set; }

        public AddInModel SelectedModel { get; set; }

        public RelayCommand Run { get; set; }

        public RelayCommand Load { get; set; }

        public RelayCommand Remove { get; set; }

        public RelayCommand ClosedEvent { get; set; }

        public AddInViewModel()
        {
            this.Models = new ObservableCollection<AddInModel>();

            this.Run = new RelayCommand(OnRun, () => SelectedModel != null && (SelectedModel.Childs == null || SelectedModel.Childs.Count == 0));
            this.Load = new RelayCommand(OnLoad);
            this.Remove = new RelayCommand(OnRemove, () => SelectedModel != null);

            this.LoadedEvent = new RelayCommand(OnLoaded);
            this.ClosedEvent = new RelayCommand(OnClosed);
        }

        private void OnClosed()
        {
            if (Models.Count == 0)
            {
                return;
            }

            try
            {
                var models = AddinAssemblyModel.Converter(Models);

                var xml = XmlHelper.Serializer(models);

                File.WriteAllText(GlobalHelper.AddInManagerAssemblyFile, xml);
            }
            catch (Exception ex)
            {

            }
        }

        private void OnLoaded()
        {
            GlobalHelper.DeleteTemp();

            if (!File.Exists(GlobalHelper.AddInManagerAssemblyFile))
            {
                return;
            }

            var xml = File.ReadAllText(GlobalHelper.AddInManagerAssemblyFile);

            if (string.IsNullOrEmpty(xml))
            {
                return;
            }

            var models = XmlHelper.Deserialize<List<AddinAssemblyModel>>(xml);

            if (models == null || models.Count == 0)
            {
                return;
            }

            this.Models = AddinAssemblyModel.Converter(models);
        }

        private void OnRemove()
        {
            if (SelectedModel.Parent == null)
            {
                Models.Remove(SelectedModel);
            }
            else if (SelectedModel.Parent.Childs != null)
            {
                SelectedModel.Childs.Remove(SelectedModel);

                if (SelectedModel.Parent.Childs.Count == 0)
                {
                    Models.Remove(SelectedModel.Parent);
                }
            }
        }

        private void OnLoad()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "请选择要加载的程序集..."
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var file = openFileDialog.FileName;

                var assembly = AssemblyDefinition.ReadAssembly(file);

                var types = assembly.Modules[0].Types.Where(x => x.HasInterfaces
                                                              && x.Interfaces.Any(n => n.FullName == typeof(ICommand).FullName))
                                                     .ToList();

                if (types.Count == 0)
                {
                    return;
                }

                var addin = Models.FirstOrDefault(x => x.Name == assembly.Name.Name);

                if (addin != null)
                {
                    Models.Remove(addin);
                }

                var model = new AddInModel() { Name = assembly.Name.Name, Path = file };

                foreach (var item in types)
                {
                    model.Childs.Add(new AddInModel()
                    {
                        Name = item.FullName,
                        Path = file,
                        Parent = model
                    });
                }

                Models.Add(model);
            }
        }

        private void OnRun()
        {
            var file = SelectedModel.Path;

            var assemblyResolveHelper = default(AssemblyResolveHelper);

            try
            {
                assemblyResolveHelper = new AssemblyResolveHelper();

                var assembly = assemblyResolveHelper.Registered(file);

                if (assembly == null)
                {
                    System.Windows.MessageBox.Show("Assembly is resolve fail , please check if the dependencies are missing !");
                    return;
                }

                var type = assembly.GetType(SelectedModel.Name);

                if (type == null)
                {
                    System.Windows.MessageBox.Show(string.Format(" not found {0} type", SelectedModel.Name));
                    return;
                }

                var ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, Type.DefaultBinder, new Type[0], null);

                if (ctor == null)
                {
                    System.Windows.MessageBox.Show(string.Format("{0} type not contain empty Constructor", SelectedModel.Name));
                    return;
                }

                var method = type.GetMethod("Execute", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                if (method == null)
                {
                    System.Windows.MessageBox.Show("not found {0} Execute");
                    return;
                }

                Messenger.Default.Send(this, "Closed.Token");

                var instance = ctor.Invoke(null);

                method.Invoke(instance, new object[] { null });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
                assemblyResolveHelper?.UnRegistered();
            }
        }
    }
}
