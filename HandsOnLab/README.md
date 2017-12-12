## Xamarin on the Rocks - Hands On Lab

Today we will build a cloud connected [Xamarin.Forms](http://xamarin.com/forms) application that will display a list of Movies. We will start by building the Models, View and ViewModels and then we will connect the app to an Azure Mobile App backend in just a few lines of code.

### Get Started

Open **Start/Movies/Movies.sln**

This solution contains 4 projects

* Movies  - PCL that will have all shared code (model, views, and view models)
* Movies.Droid - Xamarin.Android application
* Movies.iOS - Xamarin.iOS application (requires a macOS build host)
* Movies.UWP - Windows 10 UWP application (requires Visual Studio 2015/2017 on Windows 10)

![Solution](https://i.imgur.com/FQxgRep.png)

The **Movies** project also has blank code files and XAML pages that we will use during the Hands on Lab.

#### NuGet Restore

All projects have the required NuGet packages already installed, so there will be no need to install additional packages during the Hands on Lab. The first thing that we must do is restore all of the NuGet packages from the internet.

This can be done by **Right-clicking** on the **Solution** and selecting **Restore NuGet packages...**

### Model

We will download details about the Movies. Open the **Movies/Model/Movie.cs** file and add the following properties to the **Movie** class:

```csharp
public string Id { get; set; }
public string Title { get; set; }
public string Year { get; set; }
public string Director { get; set; }
public string Country { get; set; }
public string Poster { get; set; }
public double Rating { get; set; }
public string Genre { get; set; }
public string Plot { get; set; }
```

### View Model

The **PageViewModelBase.cs** will provide all of the functionality to display data in our our main Xamarin.Forms Page. It will consist of a list of movies and a method that can be called to get the movies from the server.

#### Implementing INotifyPropertyChanged

*INotifyPropertyChanged* is important for data binding in MVVM Frameworks. This is an interface that, when implemented, lets our view know about changes to the model.

Update:

```csharp
public abstract class ViewModelBase
{
}
```

to

```csharp
public abstract class ViewModelBase : INotifyPropertyChanged
{
    #region INotifyPropertyChanged implementation

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    #endregion
}
```
Right click and tap **Implement Interface**, which will add the following line of code:

```csharp
public event PropertyChangedEventHandler PropertyChanged;
```

We will code the helper methods **OnPropertyChanged** and **Set** that will raise the **PropertyChanged** event (see below). We will invoke the helper method whenever a property changes.

```csharp
public void OnPropertyChanged(string name = "")
{
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
```

```csharp
protected bool Set<T>(ref T field, T value, [CallerMemberName] string name = null)
{
    if (Equals(field, value))
        return false;

    field = value;
    OnPropertyChanged(name);
    return true;
}
```

```csharp
public class PageViewModelBase : ViewModelBase
{
    // Shared fields
}
```

```csharp
public class MainPageViewModel : PageViewModelBase
{
    // MainPage specific
}
```

As we will see next, whenever we want to raise a property binding update, we call the helper method Set.

PageViewModelBase will be our base class for every PageViewModel. Here we can store fields, properties and method that will be used across every PageViewModel (e.g. IsLoading property and a service to sync movies with server)

#### IsLoading
In **PageViewModelBase**, we will create a backing field and accessors for a boolean property. This will let our view know that our view model is busy so we don't perform duplicate operations (like allowing the user to refresh the data multiple times).

First, create the backing field:

```csharp
private bool _isLoading;
```

Next, create the property:

```csharp
public bool IsLoading
{
    get => _isLoading;
    set => Set(ref _isLoading, value);
}
```

Notice that we call **Set<T>();** when the value changes. The Xamarin.Forms binding infrastructure will subscribe to our **PropertyChanged** event so the UI will be notified of the change.

#### ObservableCollection of Movies

Again in **PageViewModelBase**, we will use an **ObservableCollection<Movie>** that will be cleared and then loaded with **Movie** objects. We use an **ObservableCollection** because it has built-in support to raise **CollectionChanged** events when we Add or Remove items from the collection. This means we don't call **OnPropertyChanged** when updating the collection.
The _allMovies field will be shared across the same instance of PageViewModels and hence it can be made static.

```csharp
private static ObservableCollection<Movie> _allMovies;

public ObservableCollection<Movie> AllMovies
{
    get => _allMovies;
    set => Set(ref _allMovies, value);
}
```

From **View > Task List**, You can now uncomment items labeled with **TODO Uncomment**.

#### Refresh Movies Command

A command is an action that can be data bounded to a View property, e.g. the Command property of a Button.
We will add this property in DetailPageViewModel in order to trigger the Push and Pull (Sync) of items from our app mobile backend. Let's do so!

Firstly, in **DetailPageViewModel.cs** add a property named **RefreshCommand**

```csharp
public Command RefreshCommand { get; }
```

After that, at the top of the constructor add the following initialization logic

```csharp
RefreshCommand = new Command(() =>
{
    MessagingCenter.Send<DetailPageViewModel>(this, "RefreshMovies");
});
```

The command action will leverage **MessagingCenter** APIs to notify the parent PageViewModel, namely MainPageViewModel, to sync backend Movies.
As a matter of fact, update the recipient code in MainPageViewModel constructor as follows:

```csharp
MessagingCenter.Subscribe<DetailPageViewModel>(this,
                "RefreshMovies", async sender => await RefreshAsync());
```

Lastly, provide the refresh logic for Movies along with available genres as follows

```csharp
IsLoading = true;
DetailPageViewModel.RefreshCommand.ChangeCanExecute();

AllMovies = new ObservableCollection<Movie>(await MoviesSource.GetMoviesAsync());
MenuPageViewModel.UpdateGenres();

IsLoading = false;
DetailPageViewModel.RefreshCommand.ChangeCanExecute();
```

Let's now add the missing piece of UI in **DetailPage.xaml**

## DetailPage - ToolbarItems and ListView

What we want here is to have a list of Movies to be displayed to the user and also a Toolbar button that will trigger the Movies refresh.

Following XAML Property Syntax, add the following piece of markup to create a ToolBarItem to be displayed in the top app bar.

```xml
<ContentPage.ToolbarItems>
    <ToolbarItem Text="Refresh" Command="{Binding RefreshCommand}">
        <ToolbarItem.Icon>
            <OnPlatform x:TypeArguments="FileImageSource">
                <On Platform="Android" Value="Assets/Refresh.png"/>
                <On Platform="Windows" Value="Assets/Refresh.png"/>
                <On Platform="iOS" Value=""/>
            </OnPlatform>
        </ToolbarItem.Icon>
    </ToolbarItem>
</ContentPage.ToolbarItems>
```

Then, let's create a ListView and a Progress Ring to be data bounded to the backing ViewModel properties.

```xml
<Grid>
    <ListView CachingStrategy="RecycleElement"
            ItemsSource="{Binding Movies}"
            RowHeight="220"
            ItemTapped="ListView_OnItemTapped"
            IsPullToRefreshEnabled="True"
            Refreshing="ListView_OnRefreshing"
            RefreshCommand="{Binding RefreshCommand}">
        <ListView.ItemTemplate>
            <DataTemplate>
                <ViewCell>
                    <ViewCell.ContextActions>
                        <MenuItem Clicked="OnDeleteItem"
                                CommandParameter="{Binding .}"
                                Text="Delete"
                                IsDestructive="True" />
                    </ViewCell.ContextActions>
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="148"/>
                            <ColumnDefinition Width="*"/>
                        </Grid.ColumnDefinitions>
                        <Image Source="{Binding Poster}"
                            HeightRequest="220"
                            Aspect="Fill"/>
                        <Grid Grid.Column="1" Padding="4">
                            <Grid.RowDefinitions>
                                <RowDefinition Height="*"/>
                                <RowDefinition Height="*"/>
                            </Grid.RowDefinitions>
                            <StackLayout>
                                <Label Text="{Binding Title}"
                                    FontSize="20"
                                    FontAttributes="Bold"/>
                                <Label Text="{Binding Director}"
                                    FontSize="16"
                                    FontAttributes="Italic"/>
                            </StackLayout>
                            <Grid Grid.Row="1"
                                VerticalOptions="End"
                                Margin="0,0,0,8">
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="4*"/>
                                    <ColumnDefinition Width="*"/>
                                </Grid.ColumnDefinitions>
                                <StackLayout Orientation="Horizontal">
                                    <Label Text="{Binding Country}" />
                                    <Label Text="-" />
                                    <Label Text="{Binding Year}" />
                                </StackLayout>
                                <Label Grid.Column="1"
                                    FontSize="16"
                                    FontAttributes="Bold"
                                    HorizontalOptions="End"
                                    Text="{Binding Rating, StringFormat='{0:0.0}'}"
                                    Margin="0,0,4,0"/>
                            </Grid>
                        </Grid>
                    </Grid>
                </ViewCell>
            </DataTemplate>
        </ListView.ItemTemplate>
    </ListView>
    <ActivityIndicator VerticalOptions="Center"
                        HorizontalOptions="Center"
                        IsRunning="{Binding IsLoading}"
                        IsVisible="{Binding IsLoading}"/>
</Grid>
```

Now the DetailPage UI is ready to display data that will come from our source, such as data coming from an Azure Mobile App.

## Connect to Azure Mobile Apps

Being able to grab data from a RESTful end point is great, but what about creating the back-end service? This is where Azure Mobile Apps comes in. Let's update our application to use an Azure Mobile Apps back-end.

If you don't already have an Azure account, go to [http://portal.azure.com](http://portal.azure.com) and register.

Once you're registered, open the Azure portal, select the **+ New** button and search for **mobile apps**. You will see the results as shown below. Select **Mobile Apps Quickstart**

![Quickstart](http://content.screencast.com/users/JamesMontemagno/folders/Jing/media/c2894f06-c688-43ad-b812-6384b34c5cb0/2016-07-11_1546.png)

The Quickstart blade will open, select **Create**

![Create quickstart](http://content.screencast.com/users/JamesMontemagno/folders/Jing/media/344d6fc2-1771-4cb7-a49a-6bd9e9579ba6/2016-07-11_1548.png)

This will open a settings blade with 4 settings:

**App name**

This is a unique name for the app that you will need when connecting your Xamarin.Forms client app to the hosted Azure Mobile App. You will need to choose a globally-unique name; for example, you could try something like *yourlastnameMovies*.

**Subscription**
Select a subscription or create a pay-as-you-go account (this service will not cost you anything).

**Resource Group**
Select *Create new* and call it **Movies**.

A resource group is logical container the can hold multiple Azure services. Using a resource group allows you to delete a collection of related services in one step.

**App Service plan/Location**
Click this field and select **Create New**, give it a unique name, select a location (typically you would choose a location close to your customers), and then select the F1 Free tier:

![service plan](http://content.screencast.com/users/JamesMontemagno/folders/Jing/media/7559d3f1-7ee6-490f-ac5e-d1028feba88f/2016-07-11_1553.png)

Finally check **Pin to dashboard** and click create:

![](http://content.screencast.com/users/JamesMontemagno/folders/Jing/media/a844c283-550c-4647-82d3-32d8bda4282f/2016-07-11_1554.png)

This will take about 3-5 minutes to setup, so let's head back to the code!


### Update AzureService.cs
We will use the [Azure Mobile Apps SDK](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-xamarin-forms-get-started/) to connect our mobile app to our Azure back-end with just a few lines of code.

Open the Movies/Services/MobileAppMoviesSource.cs and add our url to the MobileAppUrl static field:

```csharp
private static readonly string MobileAppUrl = "https://OUR-APP-NAME-HERE.azurewebsites.net";
```

Be sure to update YOUR-APP-NAME-HERE with the app name you specified when creating your Azure Mobile App.

The logic in the Initialize method will setup our database and create our `IMobileServiceSyncTable<Movie>` table that we can use to retrieve Movie data from the Azure Mobile App. There are two methods that we need to fill in to get and sync data from the server.


#### GetMovies
In this method, we will need to initialize, sync, and query the table for items. We can use complex LINQ queries to order the results:

```csharp
await InitAndSync();
await _moviesTable.OrderBy(m => m.Genre).ToListAsync();
```

#### SyncMovies
Our Azure backend can push any local changes and then pull all of the latest data from the server using the following code that can be added to the try inside of the SyncMovies method:

```csharp
ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;
try
{
    await _mobileServiceClient.SyncContext.PushAsync();
}
catch (MobileServicePushFailedException e)
{
    if (e.PushResult != null)
    {
        syncErrors = e.PushResult.Errors;
    }
}

// Error/conflict handling.
if (syncErrors != null)
{
    foreach (var error in syncErrors)
    {
        if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
        {
            // Revert to server's copy
            await error.CancelAndUpdateItemAsync(error.Result);
        }
        else
        {
            // Discard local change
            await error.CancelAndDiscardItemAsync();
        }

        Debug.WriteLine($"Error executing sync operation. Item: {error.TableName} ({error.Item["id"]}). Operation discarded.");
    }
}

try
{
    await _moviesTable.PullAsync("allMovies", _moviesTable.CreateQuery());
}
catch (Exception )
{
    // It is ok if we are not connected
}
```
That is it for our Azure code! Just a few lines, and we are ready to pull the data from Azure.

Now, we have implemented the code we need in our app! Amazing isn't it?The AzureService object will automatically handle all communication with your Azure back-end for you, do online/offline synchronization so your app works even when it's not connected.

Let's head back to the Azure Portal and populate the database.

When the Quickstart finishes you should see the following screen, or can go to it by tapping the pin on the dashboard:

![Quickstart](http://content.screencast.com/users/JamesMontemagno/folders/Jing/media/71ad3e06-dcc5-4c8b-8ebd-93b2df9ea2b2/2016-07-11_1601.png)

Under **Features** select **Easy Tables**.

It will have created a `TodoItem`, which you should see, but we can create a new table and upload a default set of data by selecting **Add from CSV** from the menu.

Ensure that you have downloaded this repo and have the **Movies.csv** file that is in this folder.

Select the file and it will add a new table name and find the fields that we have listed. Then hit Start Upload.

![upload data](http://content.screencast.com/users/JamesMontemagno/folders/Jing/media/eea2bca6-2dd0-45b3-99af-699d14a0113c/2016-07-11_1603.png)

Now you can re-run your application and get data from Azure!


## Take Home Challenges

### Challenge 1: Text to Speech service

In this challenge you will add a new feature in our Movie app, that is Text to Speech, a service that is provided in every major platform.
However, the Xamarin.Forms framework does not expose such abstracted service and to carry out this challenge you will have to provide plarform specific implementation.

1.) In **MoviePage.cs** Summary page, add a ToolbarItem button and bind it to the MoviePageViewModel TextToSpeechCommand.

2.) Initialize **TextToSpeechCommand** action by calling the ITextToSpeech interface service. You will notice that there are no services implementing such interface. As a matter of fact, you will have to create a TextToSpeechNative service in each platform-specific project and inject it through the Xamarin.Forms DependencyService.
As a reference, have a look at [DependencyService](https://developer.xamarin.com/guides/xamarin-forms/application-fundamentals/dependency-service/introduction/).

### Challenge 2: Add a MovieAddPage

In this challenge you will create a form for adding a new Movie to the list of synchronized Movies of the app. In fact, you may have noticed that the AddMoviePage is currenly empty. The corresponding ViewModel is instead ready to be data bounded.

1.) In **DetailPage.cs**, add a new ToolbarItem button to navigate to the **AddMoviePage** (assets provided)

2.) Add all the textbox needed to store a new Movie:
- Title
- Year
- Director
- Country
- Poster
- Rating
- Genre
- Plot

For each of these visual elements, you will have to hook up its two-way binding.

3.) Add a Save ToolbarItem button (assets provided)

Happy coding!