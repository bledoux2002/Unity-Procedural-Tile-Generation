# Unity Procedural Tile Generation
 A project focused on developing a tool for custom tilemap generation, allowing for control over path, terrain, and structure spawn rates.

NOTES
- For simulation of extra layers, generate (but do not assign, add to 2d array) in a while loop, return false bool if error occurs, while success == false, keep simulating, after only generate saved list of valid tiles at their coords
