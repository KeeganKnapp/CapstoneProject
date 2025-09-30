## 📂 Project Overview

root/  
├─ CapstoneMaui/ &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp; # .NET MAUI app (Android/Windows/iOS)  
│ &emsp; ├─ App.xaml, MainPage.razor &emsp;&emsp;&emsp;&emsp;&emsp;&emsp; # UI & navigation  
│ &emsp; ├─ MauiProgram.cs &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp; # DI container & app startup  
│ &emsp; ├─ Platforms/ &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp; # platform specifics  
│ &emsp; └─ Components  
│ &emsp;&emsp; ├─ Layout &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp; #contains main layout including overhead appbar  
│ &emsp;&emsp; └─ Pages  &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp; #contains each page  
│
├─ Capstone.Core/ &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp; # UI-agnostic logic  
│ &emsp; ├─ Abstractions/ &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp; # public interfaces (IAuthService, ITimeEntryService, ...)  
│ &emsp; ├─ Clients/ &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp; # API clients (HttpClient/Refit/fetch logic)  
│ &emsp; ├─ Services/ &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp; # business services (compose Clients + domain rules)  
│ &emsp; ├─ Contracts/ &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp; # DTOs / request/response models  
│ &emsp; ├─ Options/ &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp; # strongly-typed config (ApiOptions, AuthOptions)  
│ &emsp; ├─ Extensions/ &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp;&nbsp; # DI extension: AddCoreServices(...)  
│ &emsp; └─ Capstone.Core.csproj  
│  
└─ tests/  
&emsp; └─ CapstoneMaui.Tests.NUnit/ &emsp;&emsp;&emsp;&emsp;&nbsp; # NUnit tests (mostly for Capstone.Core)  
&emsp;&emsp;&emsp; ├─ Mocks/ &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp; # test doubles or AutoFixture customizations  
&emsp;&emsp;&emsp; ├─ Services/ &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp; # unit tests for Services  
&emsp;&emsp;&emsp; ├─ Clients/ &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp;&nbsp; # unit tests for API client logic  
&emsp;&emsp;&emsp; └─ Test.csproj  


