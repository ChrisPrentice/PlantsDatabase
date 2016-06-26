using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using PlantDatabaseRepositories;
using PlantsDatabase.IRIS.Law.SharedWpf;

namespace PlantsDatabase
{
    public class PlantViewModel :BaseViewModel
    {
        private string _plantName;
        private ObservableCollection<Plant> _plants;
        private readonly PlantsRepository _plantRepo;
        private int _selectedPlantTypeId = 1;

        public PlantViewModel()
        {
            _plantRepo = new PlantsRepository();
            var collection = _plantRepo.GetPlants();
            _plants = new ObservableCollection<Plant>(collection);

            AddPlantCommand = new DelegateCommand(() =>
            {
                _plantRepo.AddPlant(_plantName, _selectedPlantTypeId);
                _plants = new ObservableCollection<Plant>(_plantRepo.GetPlants());
                OnPropertyChanged("Plants");
            });

            GoogleCommand = new DelegateCommand<Plant>(p =>
            {
                string googleLink = ImageSearchUriCreator.Create(p.PlantTypeLatinName, p.PlantName);
                Process.Start(googleLink);
            });
        }

        public ICommand AddPlantCommand { get; set; }
        public ICommand GoogleCommand { get; set; }
        public string SiteHeader => "Plants Database";

        public string PlantName
        {
            get { return _plantName; }
            set { SetValue(ref _plantName, value, "PlantName"); }
        }

        public ObservableCollection<Plant> Plants
        {
            get { return _plants; }
            set { SetValue(ref _plants, value, "Plants"); }
        }

        public IEnumerable<PlantType> PlantTypes => _plantRepo.GetPlantTypes();
        public IEnumerable<PlantFamily> PlantFamilies => _plantRepo.GetPlantFamilies();

        public int SelectedPlantTypeId
        {
            get { return _selectedPlantTypeId; }
            set { SetValue(ref _selectedPlantTypeId, value, "SelectedPlantTypeId"); }
        }
    }
}