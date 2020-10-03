# NARS-Visualization
NARS graphical visualization tool

Requires modified NARS to output a line with a specific prefix whenever the memory structure is updated. When a line with the prefix is detected, the visualizer will update with corresponding visuals

(FORMAT) Memory Update : Shell Output
------------------------------
- New Concept added to memory: "New Concept: " + term
- New Inheritance relation: "New Inherit: " + subject_term + "-->" + predicate_term


Example
-------------------------------------
Visualizer after 2 inputs:
- <raven --> bird>.
- <bird --> animal>.

![Capture](https://user-images.githubusercontent.com/15344554/95002493-aec63c00-05a2-11eb-8f95-061b32b75bb1.PNG)
