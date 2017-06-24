using FontAwesome.WPF;
using Launcher.exceptions;
using Launcher.models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Launcher {

    public partial class MainWindow : Window {

        private List<string> updateList = new List<string>();
        private List<News> newsList;
        private List<Ranking> rankingList;

        /**
         * Inicializa la interfáz y pone a correr las tareas necesarias
         */
        public MainWindow() {
            InitializeComponent();
            try {
                // Si el launcher solo puede tener un Main abierto a la vez, y ya hay uno abierto, se cierra el launcher
                if (Process.GetProcessesByName("Main").Count() > 0 && Constants.ONLY_ONE_MAIN) {
                    Process.GetCurrentProcess().Kill();
                    // Si hay más de un launcher abierto a la vez, este se cierra
                } else if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Constants.LAUNCHER_PATH)).Count() > 1) {
                    Process.GetCurrentProcess().Kill();
                    // Si el launcher no se inició como administrador, este se cierra
                } else if (!Utils.isUserAdministrator()) {
                    throw new UserWithoutRightsException("Debes ejecutar el launcher como administrador");
                    // Si el launcher no está en la carpeta del main, este se cierra
                } else if (!checkMain()) {
                    throw new InvalidDirectoryException("El launcher debe ubicarse en la carpeta del juego");
                } else {
                    cleanOldLauncher();
                    prepareGUI();
                    BackgroundWorker updateWorker = new BackgroundWorker();
                    updateWorker.WorkerReportsProgress = true;
                    updateWorker.DoWork += new DoWorkEventHandler(updateWorker_DoWork);
                    updateWorker.ProgressChanged += new ProgressChangedEventHandler(updateWorker_ProgressChanged);
                    updateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(updateWorker_RunWorkerCompleted);
                    updateList = getFiles();
                    progressBar.Maximum = updateList.Count;
                    BackgroundWorker newsWorker = new BackgroundWorker();
                    newsWorker.DoWork += new DoWorkEventHandler(newsWorker_DoWork);
                    newsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(newsWorker_RunWorkerCompleted);
                    BackgroundWorker rankingWorker = new BackgroundWorker();
                    rankingWorker.DoWork += new DoWorkEventHandler(rankingWorker_DoWork);
                    rankingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(rankingWorker_RunWorkerCompleted);
                    updateWorker.RunWorkerAsync();
                    newsWorker.RunWorkerAsync();
                    rankingWorker.RunWorkerAsync();
                }
            } catch (UserWithoutRightsException ex) {
                MessageBox.Show("Debes ejecutar el launcher como administrador");
                Utils.log(ex.ToString());
                Close();
            } catch (InvalidDirectoryException ex) {
                MessageBox.Show("El launcher debe ubicarse en la carpeta del juego");
                Utils.log(ex.ToString());
                Close();
            } catch (Exception ex) {
                Utils.log(ex.ToString());
            }
        }

        /**
         * Modifica el método por defecto de click derecho para poder hacer Drag and Drop de la aplicación
         */
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);
            try {
                DragMove();
            } catch (Exception ex) {
                Utils.log(ex.ToString());
            }
        }

        /**
         * Background Worker del AutoUpdater
         */
        private void updateWorker_DoWork(object sender, DoWorkEventArgs e) {
            int count = 1;
            foreach (string archivo in updateList) {
                try {
                    string archivoPath = archivo.Substring(0, archivo.IndexOf('|'));
                    string md5Hash = archivo.Substring(archivo.IndexOf('|') + 1);

                    if (!checkFile(archivoPath, md5Hash)) {
                        downloadFile(archivoPath);
                    }
                    BackgroundWorker bWorker = (BackgroundWorker)sender;
                    bWorker.ReportProgress(count++, archivoPath);
                } catch (Exception ex) {
                    Utils.log(ex.ToString());
                } 
            }
        }

        void updateWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate () {
                progressBar.Value = e.ProgressPercentage;
                label.Content = progressBar.Value + " / " + progressBar.Maximum;
            });
        }

        private void updateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate () {
                label.Content = "Listo para jugar";
                BTNClose.IsEnabled = true;
                BTNOptions.IsEnabled = true;
                BTNPlay.IsEnabled = true;
            });
        }

        /**
         * Evento Click del botón de cierre
         */
        private void BTNClose_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        /**
         * Evento Click del botón de jugar
         */
        private void BTNPlay_Click(object sender, RoutedEventArgs e) {
            openMain();
            Close();
        }

        /**
         * Evento Click del botón de opciones
         */
        private void BTNOptions_Click(object sender, RoutedEventArgs e) {
            try {
                Options a = new Options();
                a.ShowDialog();
            } catch (Exception ex) {
                Utils.log(ex.ToString());
            }
        }

        /**
         * Comprueba si el MAIN existe
         */
        public bool checkMain() {
            if (!(new FileInfo(Constants.MAIN_PATH)).Exists) {
                return false;
            } else {
                return true;
            }
        }
        
        /**
         * Obtiene los archivos para actualizar y los retorna en una Lista
         */
        public List<String> getFiles() {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(Constants.UPDATES_LIST);
            StreamReader reader = new StreamReader(stream);
            string line;
            List<String> files = new List<String>();

            while ((line = reader.ReadLine()) != null) {
                files.Add(line);
            }
            return files;
        }

        /**
         * Retorna TRUE si el archivo en internet coincide con el actual
         */
        public bool checkFile(String path, String hash) {
            String completePath = Constants.PATH + path.Replace(".gz", string.Empty);
            FileInfo file = new FileInfo(completePath);
            if (file.Exists) {
                if (Utils.compareMD5(hash, completePath)) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        /**
         * Descarga el archivo de actualización y lo descomprime
         */
        public void downloadFile(String path) {
            using (var client = new WebClient()) {
                FileInfo file = new FileInfo(Constants.PATH + path);
                if (!file.Directory.Exists) {
                    file.Directory.Create();
                }
                client.DownloadFile(new Uri(Constants.UPDATES_LOCATION + path), Constants.PATH + path);
                if (!path.Contains("mu.exe.gz")) {
                    uncompress(Constants.PATH + path);
                } else {
                    uncompressLauncher(Constants.PATH + path);
                }
            }
        }

        /**
         * Descomprime el archivo y borra el archivo comprimido
         */
        public void uncompress(String path) {
            FileInfo zipFile = new FileInfo(path);
            Utils.unzip(path);
            zipFile.Delete();
        }

        /**
         * Descomprime el launcher nuevo, renombra el viejo a {NOMBRE}.bak, abre el nuevo y cierra el viejo
         */
        public void uncompressLauncher(String path) {
            File.Move(Constants.LAUNCHER_PATH, Constants.LAUNCHER_PATH + ".bak");
            FileInfo zipFile = new FileInfo(path);
            Utils.unzip(path);
            zipFile.Delete();
            System.Diagnostics.Process.Start(Constants.LAUNCHER_PATH);
            Process.GetCurrentProcess().Kill();
        }

        /**
         * Ejecuta el main y hace el cambio en la memoria correspondiente
         */
        public void openMain() {
            try {
                Process process = Process.Start(Constants.MAIN_PATH);
                Utils.WriteMem(process, 0x004D1E69, 0xEB);
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        /**
         * Limpia el viejo cliente
         */
        private void cleanOldLauncher() {
            String filePath = Constants.LAUNCHER_PATH + ".bak";
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
        }

        /**
         * BackgroundWorker del API de noticias
         */
        private void newsWorker_DoWork(object sender, DoWorkEventArgs e) {
            using (WebClient client = new WebClient()) {
                try {
                    String newsFromWeb = client.DownloadString(Constants.API_NEWS_WEB);
                    JObject newsJSON = JObject.Parse(newsFromWeb);
                    List<JToken> results = newsJSON["news"].Children().ToList();
                    if (results.Count > 0) {
                        this.newsList = new List<News>();
                        foreach (JToken result in results) {
                            News n = result.ToObject<News>();
                            this.newsList.Add(n);
                        }
                    }
                } catch (Exception ex) {
                Utils.log(ex.ToString());
                }
            }
        }

        private void newsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate () {
                try {
                    if (newsList.Count > 0) {
                        newsGrid.Children.Clear();
                        newsGrid.Children.Add(createNewsTitle());
                        int i = 1;
                        foreach (News noticia in newsList) {
                            // Creamos un nuevo panel para insertar el ícono FA con el título de la noticia
                            StackPanel stackNews = new StackPanel();
                            stackNews.Orientation = Orientation.Horizontal;
                            Label newsLabel = createNewsLabel();
                            newsLabel.Content = noticia.title;
                            newsLabel.Tag = Constants.NEWS_WEB + noticia.id;
                            stackNews.Children.Add(createNewspaperIcon());
                            stackNews.Children.Add(newsLabel);
                            Grid.SetRow(stackNews, i++);
                            newsGrid.Children.Add(stackNews);
                        }
                    }
                } catch (Exception ex) {
                    Utils.log(ex.ToString());
                }
            });
        }

        /**
         * BackgroundWorker del API de Ranking
         */
        private void rankingWorker_DoWork(object sender, DoWorkEventArgs e) {
            using (WebClient client = new WebClient()) {
                try {
                    String rankingFromAPI = client.DownloadString(Constants.API_RANKING);
                    JObject rankingJSON = JObject.Parse(rankingFromAPI);
                    List<JToken> results = rankingJSON["ranking"].Children().ToList();
                    if (results.Count > 0) {
                        this.rankingList = new List<Ranking>();
                        foreach (JToken result in results) {
                            Ranking n = result.ToObject<Ranking>();
                            this.rankingList.Add(n);
                        }
                    }
                } catch (Exception ex) {
                    Utils.log(ex.ToString());
                }
            }
        }

        private void rankingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate () {
                try {
                    if (rankingList.Count > 0) {
                        rankingGrid.Children.Clear();
                        Label numero = createRankingLabel("#", 0, 0);
                        numero.Foreground = Utils.brushFromHTML("#f25454");
                        Label nombre = createRankingLabel("NOMBRE", 0, 1);
                        nombre.Foreground = Utils.brushFromHTML("#f25454");
                        Label nivel = createRankingLabel("LEVEL", 0, 2);
                        nivel.Foreground = Utils.brushFromHTML("#f25454");
                        Label resets = createRankingLabel("RR", 0, 3);
                        resets.Foreground = Utils.brushFromHTML("#f25454");
                        rankingGrid.Children.Add(numero);
                        rankingGrid.Children.Add(nombre);
                        rankingGrid.Children.Add(nivel);
                        rankingGrid.Children.Add(resets);
                        int i = 1;
                        foreach (Ranking puesto in rankingList) {
                            Label posicion = createRankingLabel(i, i, 0);
                            posicion.Foreground = Utils.brushFromHTML("#f25454");
                            rankingGrid.Children.Add(posicion);
                            rankingGrid.Children.Add(createRankingLabel(puesto.Name, i, 1));
                            rankingGrid.Children.Add(createRankingLabel(puesto.Level, i, 2));
                            rankingGrid.Children.Add(createRankingLabel(puesto.Resets, i++, 3));
                        }
                    }
                } catch (Exception ex) {
                    Utils.log(ex.ToString());
                }
            });
        }

        /**
         * Crea un label para insertar en la tabla de ranking
         */
        private Label createRankingLabel(Object content, int row, int column) {
            Label label = new Label();
            label.Content = content;
            label.Foreground = Utils.brushFromHTML("#ffffff");
            label.FontFamily = new FontFamily("Roboto");
            label.FontSize = 25;
            label.FontWeight = FontWeights.Normal;
            Grid.SetColumn(label, column);
            Grid.SetRow(label, row);
            return label;
        }

        /**
         * Crea un label para insertar como noticia
         */
        private Label createNewsLabel() {
            Label label = new Label();
            label.Foreground = Utils.brushFromHTML("#c33d2b");
            label.FontFamily = new FontFamily("Roboto");
            label.FontSize = 30;
            label.FontWeight = FontWeights.Normal;
            label.MouseEnter += new MouseEventHandler(newsLabel_MouseEnter);
            label.MouseLeave += new MouseEventHandler(newsLabel_MouseLeave);
            label.MouseLeftButtonDown += new MouseButtonEventHandler(newsLabel_MouseLeftButtonDown);
            return label;
        }

        /**
         * Crea el ícono de FontAwesome para insertar al lado de la noticia
         */
        private Image createNewspaperIcon() {
            ImageSource iconSource = ImageAwesome.CreateImageSource(FontAwesomeIcon.NewspaperOutline, Utils.brushFromHTML("#c33d2b"));
            Image icon = new Image();
            icon.Source = iconSource;
            icon.Height = 10;
            icon.Margin = new Thickness(0, 10, 0, 0);
            return icon;
        }

        /**
         * Crea el label de titulares
         */
        private Label createNewsTitle() {
            Label newsTitle = new Label();
            newsTitle.Content = "NOTICIAS";
            newsTitle.FontFamily = new FontFamily("Roboto");
            newsTitle.FontSize = 30;
            newsTitle.FontWeight = FontWeights.Normal;
            newsTitle.Foreground = Utils.brushFromHTML("#ebb22d");
            newsTitle.Margin = new Thickness(newsGrid.Height / 2, 0, 0, 0);
            Grid.SetColumn(newsTitle, 0);
            Grid.SetRow(newsTitle, 0);
            return newsTitle;
        }

        /**
         * Evento MouseEnter de los label de noticias
         */
        private void newsLabel_MouseEnter(object sender, MouseEventArgs e) {
            Label s = (Label)sender;
            s.Margin = new Thickness(20, 0, 0, 0);
            s.Cursor = Cursors.Hand;
        }

        /**
         * Evento MouseLeave de los label de noticias
         */
        private void newsLabel_MouseLeave(object sender, MouseEventArgs e) {
            Label s = (Label)sender;
            s.Margin = new Thickness(0, 0, 0, 0);
            s.Cursor = Cursors.Arrow;
        }

        /**
         * Eventto Click de los label de noticias
         */
        private void newsLabel_MouseLeftButtonDown(object sender, MouseEventArgs e) {
            Label s = (Label)sender;
            Process.Start(new ProcessStartInfo((String)s.Tag));
        }

        /**
         * Prepara la interfaz para que esté en el centro de la escena y seleccionada. 
         */
        private void prepareGUI() {
            Activate();
            Focus();
            Topmost = true;
            Topmost = false;
        }

    }
}

