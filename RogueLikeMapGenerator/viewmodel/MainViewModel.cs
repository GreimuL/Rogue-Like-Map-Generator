using RogueLikeMapGenerator.model;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace RogueLikeMapGenerator.viewmodel
{
    class MainViewModel : INotifyPropertyChanged
    {
        DebugLogModel debugLogModel = new DebugLogModel();
        public string debugLog
        {
            get
            {
                return debugLogModel.debugLog;
            }
            set
            {
                debugLogModel.AddDebugLog(value);
                OnPropertyChanged("debugLog");
            }
        }
        GameMapModel gameMapModel = new GameMapModel();
        public string width
        {
            get { return gameMapModel.width.ToString(); }
            set { gameMapModel.width = int.Parse(value); }
        }
        public string height
        {
            get { return gameMapModel.height.ToString(); }
            set { gameMapModel.height = int.Parse(value); }
        }
        public string minWidth
        {
            get { return gameMapModel.minWidth.ToString(); }
            set { gameMapModel.minWidth = int.Parse(value); }
        }
        public string minHeight
        {
            get { return gameMapModel.minHeight.ToString(); }
            set { gameMapModel.minHeight = int.Parse(value); }
        }
        public string gameMapString
        {
            get { return gameMapModel.gameMapString; }
            set
            {
                gameMapModel.gameMapString = value;
                OnPropertyChanged("gameMapString");
            }
        }
        private bool isExportFinish { get; set; } = true;


        public RelayCommand generateCommand { get; set; }
        public AsyncRelayCommand exportCommand { get; set; }

        public MainViewModel()
        {
            generateCommand = new RelayCommand(GenerateGameMap, CheckGenerateAvailable);
            exportCommand = new AsyncRelayCommand(ExportGameMap, CheckExportAvailable);
        }

        /// <summary>
        /// 맵 생성
        /// </summary>
        public void GenerateGameMap(object param)
        {
            try
            {
                if (!CheckInputValid())
                {
                    return;
                }
                gameMapString = gameMapModel.GenerateGameMap();
                debugLog = "width: " + width + " height: " + height + " minWidth: " + minWidth + " minHeight: " + minHeight;
                debugLog = "Generating finish.";
            }
            catch (Exception e)
            {
                debugLog = "Fail to generate map";
                debugLog = e.Message;
            }
        }

        /// <summary>
        /// 생성된 맵을 파일로 추출
        /// </summary>
        public async Task ExportGameMap(object param)
        {
            try
            {
                debugLog = "Export Start.";
                isExportFinish = false;
                await FileExportModel.WriteFile(gameMapModel.gameMapString);
                debugLog = "Export Finish.";
            }
            catch (Exception e)
            {
                debugLog = "Export Exception";
                debugLog = e.Message;
            }
            finally
            {
                isExportFinish = true;
            }
        }

        /// <summary>
        /// 맵 생성 시 입력된 정보가 valid한지 체크
        /// </summary>
        public bool CheckInputValid(object param = null)
        {
            if (gameMapModel.width <= 0 || gameMapModel.height <= 0)
            {
                debugLog = "(width and height) can't be smaller than 1";
                return false;
            }
            if (gameMapModel.width > 5000 || gameMapModel.height > 5000)
            {
                debugLog = "(width and height) can't be larger than 5000";
                return false;
            }
            if (gameMapModel.minWidth <= 0 || gameMapModel.minHeight <= 0)
            {
                debugLog = "(minWidth and minHeight) can't be smaller than 1";
                return false;
            }
            if (gameMapModel.minWidth > gameMapModel.width - gameMapModel.roomMargin * 2 || gameMapModel.minHeight > gameMapModel.height - gameMapModel.roomMargin * 2)
            {
                debugLog = "(minWidth and minHeight) can't be larger than ((width and height) - roomMargin*2)";
                debugLog = "current roomMargin value: " + gameMapModel.roomMargin;
                return false;
            }
            return true;
        }
        /// <summary>
        /// 맵 생성 버튼 사용 가능한지 체크
        /// </summary>
        public bool CheckGenerateAvailable(object param)
        {
            if (!isExportFinish)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 맵 추출 버튼 사용 가능한지 체크
        /// </summary>
        public bool CheckExportAvailable(object param)
        {
            if (gameMapModel.gameMapString == null || !isExportFinish)
            {
                return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
