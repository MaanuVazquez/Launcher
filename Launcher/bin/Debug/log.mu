
Log Entry : 6:55:48 PM Thursday, June 22, 2017
  :
  :Launcher.exceptions.InvalidDirectoryException: El launcher debe ubicarse en la carpeta del juego
   at Launcher.MainWindow..ctor() in E:\Datos\Programacion\MU ONLINE\MuEurus\Source\Launcher\Launcher\MainWindow.xaml.cs:line 51
-------------------------------

Log Entry : 6:57:48 PM Thursday, June 22, 2017
  :
  :System.InvalidOperationException: The calling thread cannot access this object because a different thread owns it.
   at System.Windows.Threading.Dispatcher.VerifyAccess()
   at System.Windows.DependencyObject.GetValue(DependencyProperty dp)
   at System.Windows.Controls.Panel.get_IsItemsHost()
   at System.Windows.Controls.ItemsControl.GetItemsOwnerInternal(DependencyObject element, ItemsControl& itemsControl)
   at System.Windows.Controls.Panel.VerifyBoundState()
   at System.Windows.Controls.Panel.get_InternalChildren()
   at System.Windows.Controls.Panel.get_Children()
   at Launcher.MainWindow.newsWorker_DoWork(Object sender, DoWorkEventArgs e) in E:\Datos\Programacion\MU ONLINE\MuEurus\Source\Launcher\Launcher\MainWindow.xaml.cs:line 267
-------------------------------

Log Entry : 7:03:16 PM Thursday, June 22, 2017
  :
  :System.ComponentModel.Win32Exception (0x80004005): The system cannot find the file specified
   at System.Diagnostics.Process.StartWithShellExecuteEx(ProcessStartInfo startInfo)
   at System.Diagnostics.Process.Start()
   at System.Diagnostics.Process.Start(ProcessStartInfo startInfo)
   at System.Diagnostics.Process.Start(String fileName)
   at Launcher.MainWindow.uncompressLauncher(String path)
   at Launcher.MainWindow.downloadFile(String path)
   at Launcher.MainWindow.bWorker_DoWork(Object sender, DoWorkEventArgs e)
-------------------------------

Log Entry : 7:34:42 PM Friday, June 23, 2017
  :
  :System.ComponentModel.Win32Exception (0x80004005): The system cannot find the file specified
   at System.Diagnostics.Process.StartWithShellExecuteEx(ProcessStartInfo startInfo)
   at System.Diagnostics.Process.Start()
   at System.Diagnostics.Process.Start(ProcessStartInfo startInfo)
   at System.Diagnostics.Process.Start(String fileName)
   at Launcher.MainWindow.uncompressLauncher(String path)
   at Launcher.MainWindow.downloadFile(String path)
   at Launcher.MainWindow.updateWorker_DoWork(Object sender, DoWorkEventArgs e)
-------------------------------
