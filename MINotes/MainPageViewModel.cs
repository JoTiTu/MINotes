using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace MINotes
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        // Commands for bindings
        public ICommand AddNoteCommand => new Command(AddNote);
        public ICommand DeleteNoteCommand => new Command(DeleteNote);

        // Property changes event
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=netcore-3.1
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Path for internal data persistence
        private readonly string FilePath = Path.Combine(System.Environment.
            GetFolderPath(System.Environment.SpecialFolder.Personal), "notes.xml");

        // Store all Notes which are in ListView
        public ObservableCollection<NoteModel> AllNotes
        {
            get;
            set;
        }

        // Bind Entry
        public string EditorInput
        {
            get;
            set;
        }

        public MainPageViewModel()
        {
            // Use persistent data
            if (File.Exists(FilePath))
            {
                AllNotes = ReadNotesFile();
            }
            // No persistent data yet
            else
            {
                AllNotes = new ObservableCollection<NoteModel>() { };
            }
        }

        public void AddNote()
        {
            AllNotes.Add(new NoteModel()
            {
                InputTxt = EditorInput,
                ImgSource = "https://picsum.photos/200/?random=" + new Random().Next().ToString()
            });
            ToNotesFile();
            EditorInput = "";
            NotifyPropertyChanged(EditorInput);
        }

        public void DeleteNote(object sender)
        {
            AllNotes.Remove(sender as NoteModel);
            ToNotesFile();
        }

        public void ToNotesFile()
        {
            // Initialise the serialiser
            var serializer = new XmlSerializer(typeof(ObservableCollection<NoteModel>));
            // Initialise the writer
            var writer = new FileStream(FilePath, FileMode.Create);
            // Write to the file
            serializer.Serialize(writer, AllNotes);
            writer.Close();
        }

        public ObservableCollection<NoteModel> ReadNotesFile()
        {
            var serializer = new XmlSerializer(typeof(ObservableCollection<NoteModel>));
            var reader = new FileStream(FilePath, FileMode.Open);
            ObservableCollection<NoteModel> deserializedNotesList;
            // Read from the file and deserialize
            deserializedNotesList = (ObservableCollection<NoteModel>)serializer.Deserialize(reader);
            reader.Close();

            return deserializedNotesList;
        }

        // Delete Notes file (used for testing)
        public void DeleteNotesFile()
        {
            File.Delete(FilePath);
        }
    }
}
