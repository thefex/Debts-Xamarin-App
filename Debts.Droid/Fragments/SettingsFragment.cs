using System;
using System.Globalization;
using System.Linq;
using Android.Graphics;
using Android.OS; 
using Android.Runtime;
using Android.Support.V14.Preferences;
using Android.Support.V7.Preferences;
using Android.Views;
using Android.Widget;
using Debts.ViewModel;
using DynamicData;
using Humanizer;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Extensions;
using MvvmCross.Droid.Support.V7.Preference;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace Debts.Droid.Fragments
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.FragmentRoot, false, EnterAnimation = Resource.Animation.abc_fade_in, ExitAnimation = Resource.Animation.abc_fade_out, PopEnterAnimation = Resource.Animation.abc_fade_out, PopExitAnimation = Resource.Animation.abc_fade_in)]
    public class SettingsFragment : MvxPreferenceFragmentCompat<SettingsViewModel>
    {
        private ListPreference currencyPreference;
        private ListPreference notificationTimeSpanPreference;
        private ListPreference notificationDeadlinePassedTimeSpanPreference;

        private SwitchPreference upcomingDebtsSwitch;
        private SwitchPreference upcomingLoansSwitch;

        private SwitchPreference unpaidDebtsSwitch;
        private SwitchPreference unpaidLoansSwitch;
        
        public SettingsFragment()
        {
            RetainInstance = true;
        }

        public SettingsFragment(System.IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            
            var generalPreferences = FindPreference("General") as PreferenceCategory;
            generalPreferences.IconSpaceReserved = false;
            var notificationsPreferences = FindPreference("Notifications") as PreferenceCategory;
            notificationsPreferences.IconSpaceReserved = false;
 
            currencyPreference = (ListPreference)FindPreference("DefaultCurrency");
            currencyPreference.SetEntryValues(ViewModel.Currencies.Select(x => x.Currency).ToArray());
            currencyPreference.SetEntries(ViewModel.Currencies.Select(x => $"{x.Value} ({x.Currency})").ToArray());
            currencyPreference.SetValueIndex(ViewModel.Currencies.IndexOf(ViewModel.SelectedCurrency));
            
            currencyPreference.PreferenceChange += CurrencyPreferenceOnPreferenceChange; 
            
            currencyPreference.Title = ($"Currency ({ViewModel.SelectedCurrency.Currency})");
 
            notificationTimeSpanPreference = (ListPreference)FindPreference("NotificationDeadlineUpcoming");
            notificationTimeSpanPreference.SetEntries(ViewModel.MinimalAmountOfTimeBeforeNotifications.Select(x =>
            {
                try
                {
                    return x.Humanize();
                }
                catch (Exception)
                {
                    return x.Humanize(culture: new CultureInfo("en-us"));
                }
            }).ToArray());
            notificationTimeSpanPreference.SetEntryValues(ViewModel.MinimalAmountOfTimeBeforeNotifications.Select(x =>
            {
                try
                {
                    return x.Humanize();
                }
                catch (Exception)
                {
                    return x.Humanize(culture: new CultureInfo("en-us"));
                }
            }).ToArray());
            notificationTimeSpanPreference.SetValueIndex(
                    ViewModel.MinimalAmountOfTimeBeforeNotifications.IndexOf(ViewModel.SelectedMinimalAmountOfTimeBeforeNotifications)
                );

            string humanizedDeadline = string.Empty;
            try
            {
                humanizedDeadline = ViewModel.SelectedMinimalAmountOfTimeBeforeNotifications.Humanize();
            }
            catch (Exception)
            {
                humanizedDeadline = ViewModel.SelectedMinimalAmountOfTimeBeforeNotifications.Humanize(culture: new CultureInfo("en-us"));
            }

            notificationTimeSpanPreference.Title = $"Deadline Approaching ({humanizedDeadline})";
            notificationTimeSpanPreference.PreferenceChange += NotificationsPreferencesOnPreferenceChange;
            
            notificationDeadlinePassedTimeSpanPreference = (ListPreference)FindPreference("NotificationDeadlinePassed");
            notificationDeadlinePassedTimeSpanPreference.SetEntries(ViewModel.MinimalAmountOfTimeBeforeNotifications.Select(x =>
            {
                try
                {
                    return x.Humanize();
                }
                catch (Exception)
                {
                    return x.Humanize(culture: new CultureInfo("en-us"));
                }
            }).ToArray());
            notificationDeadlinePassedTimeSpanPreference.SetEntryValues(ViewModel.MinimalAmountOfTimeBeforeNotifications.Select(x =>
            {
                try
                {
                    return x.Humanize();
                }
                catch (Exception)
                {
                    return x.Humanize(culture: new CultureInfo("en-us"));
                }
            }).ToArray());
            notificationDeadlinePassedTimeSpanPreference.SetValueIndex(
                ViewModel.MinimalAmountOfTimeBeforeNotifications.IndexOf(ViewModel.SelectedMinimalAmountOfTimeAfterDeadlineExceedNotifications)
            );

            humanizedDeadline = string.Empty;
            try
            {
                humanizedDeadline = ViewModel.SelectedMinimalAmountOfTimeAfterDeadlineExceedNotifications.Humanize();
            }
            catch (Exception)
            {
                humanizedDeadline = ViewModel.SelectedMinimalAmountOfTimeAfterDeadlineExceedNotifications.Humanize(culture: new CultureInfo("en-us"));
            }

            notificationDeadlinePassedTimeSpanPreference.Title = $"Deadline Exceed ({humanizedDeadline})";
            notificationDeadlinePassedTimeSpanPreference.PreferenceChange += NotificationDeadlinePassedTimeSpanPreferenceOnPreferenceChange;
            
            
            ListView.SetBackgroundColor(Color.White);
 
            upcomingDebtsSwitch = (SwitchPreference) FindPreference("notifications_approaching_debt");
            upcomingDebtsSwitch.PreferenceChange += (e, a) =>
            {
                var newValue = a.NewValue as Java.Lang.Boolean;
                ViewModel.UpcomingDebtsNotificationsEnabled = newValue?.BooleanValue() ?? false;
            };
            upcomingDebtsSwitch.Checked = ViewModel.UpcomingDebtsNotificationsEnabled;
             
            upcomingLoansSwitch = (SwitchPreference) FindPreference("notifications_approaching_loan");
            upcomingLoansSwitch.PreferenceChange += (e, a) =>
            {
                var newValue = a.NewValue as Java.Lang.Boolean;
                ViewModel.UpcomingLoansNotificationsEnabled = newValue?.BooleanValue() ?? false;
            };
            upcomingLoansSwitch.Checked = ViewModel.UpcomingLoansNotificationsEnabled;
            
            unpaidDebtsSwitch = (SwitchPreference) FindPreference("notifications_unpaid_debt");
            unpaidDebtsSwitch.PreferenceChange += (e, a) =>
            {
                var newValue = a.NewValue as Java.Lang.Boolean;
                ViewModel.UnpaidDebtsNotificationsEnabled = newValue?.BooleanValue() ?? false;
            };
            unpaidDebtsSwitch.Checked = ViewModel.UnpaidDebtsNotificationsEnabled;

            unpaidLoansSwitch = (SwitchPreference) FindPreference("notifications_unpaid_loan");
            unpaidLoansSwitch.PreferenceChange += (e, a) =>
            {
                var newValue = a.NewValue as Java.Lang.Boolean;
                ViewModel.UnpaidLoansNotificationsEnabled = newValue?.BooleanValue() ?? false;
            };
            unpaidLoansSwitch.Checked = ViewModel.UnpaidLoansNotificationsEnabled;

            var rateApp = FindPreference("RateAppButton");
            rateApp.PreferenceClick += (e, a) => { ViewModel.RateApp.Execute(); };

            var goPremium = FindPreference("GoPremiumButton");
            goPremium.Visible = ViewModel.IsGoPremiumEnabled;
            goPremium.PreferenceClick += (e, a) => { ViewModel.GoPremium.Execute(); };

            var editTextPreference = FindPreference("DisplayNamePreference");
            editTextPreference.Title = "Display name " + "(" + ViewModel.DisplayName + ")";
            editTextPreference.PreferenceChange += (e, a) =>
            {
                var newValue = a.NewValue as Java.Lang.String;
                ViewModel.DisplayName = a.NewValue.ToString();
                editTextPreference.Title = "Display name " + "(" + ViewModel.DisplayName + ")";
            };
            
            return view;
        }

        private void NotificationDeadlinePassedTimeSpanPreferenceOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            var index = notificationDeadlinePassedTimeSpanPreference.FindIndexOfValue(e.NewValue.ToString());
            ViewModel.SelectedMinimalAmountOfTimeAfterDeadlineExceedNotifications =
                ViewModel.MinimalAmountOfTimeBeforeNotifications.ElementAt(index);
            string humanized = string.Empty;
            try
            {
                humanized = ViewModel.SelectedMinimalAmountOfTimeAfterDeadlineExceedNotifications.Humanize();
            }
            catch (Exception)
            {
                humanized = ViewModel.SelectedMinimalAmountOfTimeAfterDeadlineExceedNotifications.Humanize(culture: new CultureInfo("en-us"));
            }

            notificationDeadlinePassedTimeSpanPreference.Title = $"Deadline Exceed ({humanized})";
        }

        private void NotificationsPreferencesOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            var index = notificationTimeSpanPreference.FindIndexOfValue(e.NewValue.ToString());
            ViewModel.SelectedMinimalAmountOfTimeBeforeNotifications =
                ViewModel.MinimalAmountOfTimeBeforeNotifications.ElementAt(index);
            string humanized = string.Empty;
            try
            {
                humanized = ViewModel.SelectedMinimalAmountOfTimeBeforeNotifications.Humanize();
            }
            catch (Exception)
            {
                humanized = ViewModel.SelectedMinimalAmountOfTimeBeforeNotifications.Humanize(culture: new CultureInfo("en-us"));
            }

            notificationTimeSpanPreference.Title = $"Deadline Approaching ({humanized})";
        }

        private void CurrencyPreferenceOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            var index = currencyPreference.FindIndexOfValue(e.NewValue.ToString());
            ViewModel.SelectedCurrency = ViewModel.Currencies.ElementAt(index);

            currencyPreference.Title = $"Currency ({ViewModel.SelectedCurrency.Currency})";
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.settings); 
        }
 
    }
}