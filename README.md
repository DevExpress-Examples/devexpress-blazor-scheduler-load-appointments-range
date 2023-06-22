<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/394308797/23.1.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1020463)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# Scheduler for Blazor - How to load appointments for visible interval only (lazy loading)

This example illustrates how to load only the required portion of appointments depending on the current View and visible interval. 

## Implementation Steps:
1. Add DxScheduler and bind it to a DxSchedulerDataStorage with AppointmentMappings specified ([Create Data Storage and Specify Mappings](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxSchedulerDataStorage#create-data-storage-and-specify-mappings)):
```razor
<DxScheduler DataStorage="@DataStorage">
...
</DxScheduler>
```
```cs
DxSchedulerDataStorage DataStorage = new DxSchedulerDataStorage()
    {
        AppointmentMappings = new DxSchedulerAppointmentMappings()
        {
            Id = "AppointmentId",
            Type = "AppointmentType",
            Start = "StartDate",
            End = "EndDate",
            Subject = "Caption",
            AllDay = "AllDay",
            Location = "Location",
            Description = "Description",
            LabelId = "Label",
            StatusId = "Status"
        }
    };
```
2. Declare variables for the [ActiveViewType](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxScheduler.ActiveViewType) and [StartDate](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxScheduler.StartDate) properties, and handle the [StartDateChanged](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxScheduler.StartDateChanged) and [ActiveViewTypeChanged](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxScheduler.ActiveViewTypeChanged) events:
```razor
<DxScheduler StartDate="currentDate"
             StartDateChanged="OnStartDateChanged"
             ActiveViewType="activeType"
             ActiveViewTypeChanged="OnActiveViewChanged"
             DataStorage="@DataStorage"
             CssClass="w-100">
             ...
```
3. Create a method that will load appointments depending on the current ActiveViewType and StartDate values (e.g. `LoadAppointments`). Calculate the end date in it. 
* Use the start and end date to query your data layer and get the required appointments.
* Use obtained data as [DxSchedulerDataStorage.AppointmentSource](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxSchedulerDataStorage.AppointmentsSource). Call [RefreshData](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxSchedulerDataStorage.RefreshData) to refresh the data storage:
```cs
 void LoadAppointments() {
        switch (activeType) {
            case SchedulerViewType.Day:
                startDate = currentDate;
                endDate = currentDate.AddDays(1);
                break;
            case SchedulerViewType.Week:
                startDate = currentDate.StartOfWeek(DayOfWeek.Sunday);
                endDate = startDate.AddDays(7);
                break;
            case SchedulerViewType.Month:
                startDate = currentDate.StartOfMonth();
                endDate = currentDate.AddMonths(1);
                break;
        }
        var newDataSource = AppointmentCollection.GetAppointments(startDate, endDate);
        DataStorage.AppointmentsSource = newDataSource;
        DataStorage.RefreshData();
    }
```
```cs
public static IEnumerable<Appointment> GetAppointments(DateTime startDate, DateTime endDate) {
            return GenerateAppointments().Where(p =>
                (p.StartDate >= startDate && p.EndDate <= endDate) ||       // start and end date are in the interval
                (p.StartDate >= startDate && p.StartDate <= endDate) ||     // start date is in the interval, but end date is not
                (p.EndDate >= startDate && p.EndDate <= endDate) ||         // end date is in the interval, but start date is not
                (p.StartDate < startDate && p.EndDate > endDate) ||         // appointment interval is larger than the selected interval 
                (p.AppointmentType != (int)SchedulerAppointmentType.OneTime)//always load all recurrent appointments
            );
        }
```
4. Call the `LoadAppointments` method in the OnInitialized, StartDateChanged, and ActiveViewTypeChanged handlers:
```cs
 protected override void OnInitialized() {
        base.OnInitialized();
        LoadAppointments();
    }
    void OnStartDateChanged(DateTime newStartDate) {
        currentDate = newStartDate;
        LoadAppointments();
    }
    void OnActiveViewChanged(SchedulerViewType newView) {
        activeType = newView;
        LoadAppointments();
    }
```


<!-- default file list -->
*Files to look at*:

* [Index.razor](./CS/T1019796/Pages/Index.razor)
* [AppointmentCollection.cs](./CS/T1019796/Data/AppointmentCollection.cs)
* [DateTimeExtensions.cs](./CS/T1019796/Utils/DateTimeExtensions.cs)
<!-- default file list end -->
