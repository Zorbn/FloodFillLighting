On update:
- Clear tileSize radius of the lightmap
- Resimulate the flood fill starting from each lightmap tile bordering the cleared area
- Resimulate the flood fill of each light in the cleared area