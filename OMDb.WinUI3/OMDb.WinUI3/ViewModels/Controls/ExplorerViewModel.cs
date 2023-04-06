using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.Core.Utils;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.ViewModels.Controls
{
    public class ExplorerViewModel : ObservableObject
    {
        private ExplorerItem root;
        public ExplorerItem Root
        {
            get { return root; }
            set
            {
                root = value;
                OnPropertyChanged(nameof(root));
            }
        }

        private ObservableCollectionEx<ExplorerItem> _currentFileInfos;

        public ObservableCollectionEx<ExplorerItem> CurrentFileInfos
        {
            get { return _currentFileInfos; }
            set
            {
                _currentFileInfos = value;
                OnPropertyChanged(nameof(CurrentFileInfos));
            }
        }


        private ExplorerItem _selectedFileInfo;

        public ExplorerItem SelectedFileInfo
        {
            get { return _selectedFileInfo; }
            set
            {
                _selectedFileInfo = value;
                OnPropertyChanged(nameof(SelectedFileInfo));
            }
        }


        private ObservableCollection<string> _pathStack = new ObservableCollection<string>();

        public ObservableCollection<string> PathStack
        {
            get { return _pathStack; }
            set
            {
                _pathStack = value;
                OnPropertyChanged(nameof(PathStack));
            }
        }

        public RelayCommand NavigationBackCommand { get; private set; }
        /*        public virtual void NavigationTo(ExplorerItem folder)
                {
                    DealWithNavigationStack(folder);
                    if (ToFolder(folder))
                    {
                        NavigationHistoryStack.ForEach((element) =>
                        {
                            element.IsCurrent = false;
                        });
                        folder.IsCurrent = true;
                        PushNavigationHistoryStack(folder);
                    }
                }
                private void NavigationBack()
                {
                    if (NavigationStack.Count == 1)
                    {
                        return;
                    }
                    NavigationStack.Pop();
                    var lastItem = NavigationStack.LastOrDefault();
                    if (lastItem == null)
                    {
                        return;
                    }
                    if (ToFolder(lastItem))
                    {

                        NavigationHistoryStack.ForEach((element) =>
                        {
                            element.IsCurrent = false;
                        });
                        lastItem.IsCurrent = true;

                        PushNavigationHistoryStack(lastItem);
                    }
                }
                public virtual bool ToFolder(ExplorerItem item)
                {
                    if (item == null || item.Path == CurrentExplorerItem.Path)
                    {
                        return false;
                    }

                    var currentExplorerItem = NavigationStack.FirstOrDefault(c => c.Path == item.Path);

                    if (currentExplorerItem == null)
                    {
                        return false;
                    }

                    CurrentExplorerItem = currentExplorerItem;
                    return true;
                }*/
    }
}
