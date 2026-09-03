from PIL import Image

# Open the source PNG
img = Image.open("logo256X256.png").convert("RGBA")

# Define standard Windows icon resolutions
icon_sizes = [(16, 16), (32, 32), (48, 48), (64, 64), (128, 128), (256, 256)]

# Save as multi-resolution ICO
img.save("favicon.ico", format="ICO", sizes=icon_sizes)