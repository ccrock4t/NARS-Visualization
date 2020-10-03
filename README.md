# NARS-Visualization
NARS graphical visualization tool

Requires modified NARS to output a line with a specific prefix whenever the memory structure is updated. When a line with the prefix is detected, the visualizer will update with corresponding visuals

NARS Source Code (Java): https://github.com/opennars/opennars

ONA Source Code (C): https://github.com/opennars/OpenNARS-for-Applications

Shell output format
------------------------------
- New Concept added to memory: "New Concept: " + term
- New Inheritance relation: "New Inherit: " + subject_term + "--->" + predicate_term


Example
-------------------------------------
Visualizer after 2 inputs:
- <raven --> bird>.
- <bird --> animal>.

![Capture](https://user-images.githubusercontent.com/15344554/95002493-aec63c00-05a2-11eb-8f95-061b32b75bb1.PNG)

![gif](https://user-images.githubusercontent.com/15344554/95003691-80029280-05af-11eb-9a9a-3228bbd82fea.gif)

