# Fix Your Animators
Author: Joshuarox100

Description: Quick Fix for Animators in Unity 2019.4.29f1. This serves as a workaround to a bug that occurs when duplicating or pasting Animator Controller subassets. When subassets are created this way, their HideFlags are improperly set to make them uninspectable. This script will correct the HideFlags of these subassets when the user attempts to inspect them.

## Installation Guide
To install, download the source code by clicking the green 'Code' button above and select 'Download ZIP'.
After you have downloaded the ZIP file, extract it into a folder in your Unity project.

## How to Use
While this script is present anywhere in your project, any States, Transitions, or other Animator Controller subassets will automatically have their HideFlags corrected if needed when you try to inspect them.

Additionally, if you'd like to sweep your entire project at once and fix **all** objects with the issue without inspecting them manually, click the option underneath 'Tools -> Joshuarox100 -> Fix Your Animators' to fix all Animator Controllers found in your project. After running this script, all States, Transitions, and other Animator Controller subassets that had the issue should be inspectable again.