﻿using System;
using System.Windows;
using System.Windows.Controls;
using EconomySimulator.WPF.ViewModels;
using EconomySimulator.WPF.Views.Pages;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace EconomySimulator.WPF.Views.Windows;

public partial class MainWindow : INavigationWindow
{
    public MainWindowViewModel ViewModel { get; }
    public MainWindow(MainWindowViewModel viewModel, IPageService pageService, INavigationService navigationService, IDialogService dialogService)
    {
        ViewModel = viewModel;
        DataContext = this;
        
        InitializeComponent();
        SetPageService(pageService);
        
        navigationService.SetNavigationControl(RootNavigation);

        Loaded += (_, _) =>
        {
            Navigate(typeof(SimulationMainView));
            dialogService.SetDialogControl(RootDialog);
        };
    }

    /// <summary>
    /// Raises the closed event.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Make sure that closing this window will begin the process of closing the application.
        Application.Current.Shutdown();
    }

    #region INavigationWindow

    public Frame GetFrame()
    {
        return RootFrame;
    }

    public INavigation GetNavigation()
    {
        return RootNavigation;
    }

    public bool Navigate(Type pageType)
    {
        return RootNavigation.Navigate(pageType);
    }

    public void SetPageService(IPageService pageService)
    {
        RootNavigation.PageService = pageService;
    }

    public void ShowWindow()
    {
        Show();
    }

    public void CloseWindow()
    {
        Close();
    }

    #endregion
}