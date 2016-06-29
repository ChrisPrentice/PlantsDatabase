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
        private int _selectedPlantFamilyId = 5;
        private string _plantTypeLatinName;
        private string _plantTypeCommonName;
        private bool _isNotEditMode = true;

        public PlantViewModel()
        {
            _plantRepo = new PlantsRepository();
            _plants = new ObservableCollection<Plant>(_plantRepo.GetPlants());

            AddPlantCommand = new DelegateCommand<object>(o =>
            {
                _plantRepo.AddPlant(_plantName, _selectedPlantTypeId);
                _plants = new ObservableCollection<Plant>(_plantRepo.GetPlants());
                OnPropertyChanged("Plants");
            }, o => _isNotEditMode);

            AddPlantTypeCommand = new DelegateCommand<object>(
                delegate { _plantRepo.AddPlantType(_plantTypeLatinName, _plantTypeCommonName, _selectedPlantFamilyId); }, o => _isNotEditMode);

            GoogleCommand = new DelegateCommand<Plant>(
                delegate(Plant p) { Process.Start(ImageSearchUriCreator.Create(p.PlantTypeLatinName, p.PlantName)); });

            EditPlantCommand = new DelegateCommand<Plant>(EditPlant);
            CancelEditCommand = new DelegateCommand(() => IsNotEditMode = true);
        }

        private void EditPlant(Plant plant)
        {
            IsNotEditMode = false;

            PlantName = plant.PlantName;
            PlantTypeLatinName = plant.PlantTypeLatinName;
            PlantTypeCommonName = plant.PlantTypeCommonName;
            SelectedPlantFamilyId = plant.PlantFamilyId;
            SelectedPlantTypeId = plant.PlantTypeId;
        }

        public ICommand AddPlantCommand { get; set; }
        public ICommand AddPlantTypeCommand { get; set; }
        public ICommand GoogleCommand { get; set; }
        public ICommand EditPlantCommand { get; set; }
        public ICommand CancelEditCommand { get; set; }
        public string SiteHeader => "Plants Database";
        public string PlantName { get { return _plantName; } set { SetValue(ref _plantName, value, "PlantName"); } }
        public bool IsNotEditMode { get { return _isNotEditMode; } set { SetValue(ref _isNotEditMode, value, "IsNotEditMode"); } }
        public string PlantTypeLatinName { get { return _plantTypeLatinName; } set { SetValue(ref _plantTypeLatinName, value, "PlantTypeLatinName"); } }
        public string PlantTypeCommonName { get { return _plantTypeCommonName; } set { SetValue(ref _plantTypeCommonName, value, "PlantTypeCommonName"); } }
        public ObservableCollection<Plant> Plants { get { return _plants; } set { SetValue(ref _plants, value, "Plants"); } }
        public IEnumerable<PlantType> PlantTypes => _plantRepo.GetPlantTypes();
        public int SelectedPlantTypeId { get { return _selectedPlantTypeId; } set { SetValue(ref _selectedPlantTypeId, value, "SelectedPlantTypeId"); } }
        public IEnumerable<PlantFamily> PlantFamilies => _plantRepo.GetPlantFamilies();
        public int SelectedPlantFamilyId { get { return _selectedPlantFamilyId; } set { SetValue(ref _selectedPlantFamilyId, value, "SelectedPlantFamilyId"); } }
    }
}