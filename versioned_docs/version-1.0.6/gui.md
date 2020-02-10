---
id: gui
title: GUI
sidebar_label: GUI
---

The GUI consists of two windows. When we start the application, the first window is shown - it contains main settings of the generator. The second window is opened after we click the _Generate_ button. It shows the progress of the generator and then the results.

## Settings window

### General settings
- **Number of layouts**: How many layouts we want to generate.
- **Random generator seed**: Seed of the random numbers generator, useful for debugging.
    - **use random**: The generator will always use different seed if checked.

### Map description file
Controls in this section are used to choose a map description for the generator.

- **Choose from existing** - Dropdown with map descriptions in the _Resources/Maps_ folder.
- **Upload your own** - Allows users to upload their own map descriptions.

### Info
After choosing a map description, this section shows information about the description.

### Progress showing settings
Layout generator provides events that are fired when for example layout is accepted. These events can be used to show the progress of the generator. The purpose of all numeric inputs in this section is to set for how long should be a given layout shown.

- **Show final layouts**: Whether final layouts should be shown. Check this if you want to see each layout right after when it is generated. This checkbox does not affect showing results after *all* layouts are generated.
- **Show partial valid layouts**: Whether to show all valid partial layouts.
- **Show all perturbed layouts**: Whether to show all perturbed layouts.
- **Export shown layouts**: All shown layouts will be exported if checked. That means that if we choose to show final and partial layouts, all these layouts will be exported. Layouts are exported as svg files and obey settings from the following section.

### Display settings
- **Show room names**: Whether we want to show room names.
- **Use old paper style**: Whether we want to use the old paper style for rendering generated layouts. This option works only with raster images, i.e. images shown directly in the GUI and PNG exports.
- **Fixed font size**: Font size of room names is usually computed from the size of the smallest room. This options allows us to set a fixed font size.
- **Fixed square export**: All layouts will be exported as squares with a specified width.

### Generate button
This button starts the generation process.

## Generator window

### Top bar
- **Export SVG**: Exports the currently shown layout as a SVG file.
- **Export JSON**: Exports the currently shown layout as a JSON file.
- **Slideshow handlers**: Iterate through all generated layouts.
    - **Automatic slideshow**: Results are shown automatically if checked.

### Info
This panel shows progress of the generator.

### Actions
- **Export all JSON**: Exports all generated layouts as a JSON file.
- **Export all JGP**: Exports all generated layouts as JPG files to the _Output_ folder.


