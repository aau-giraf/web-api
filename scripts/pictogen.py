#!/usr/bin/env python3

from PIL import Image, ImageDraw, ImageFont
import textwrap, sys, os

pictograms = 100
output = "../pictograms"

def write(id, outdir):
    astr = str(id)
    para = textwrap.wrap(astr, width=15)

    MAX_W, MAX_H = 200, 200
    im = Image.new('RGB', (MAX_W, MAX_H), 'white')
    draw = ImageDraw.Draw(im)
    font = ImageFont.truetype('/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf', 36)

    for line in para:
        w, h = draw.textsize(line, font=font)
        draw.text(((MAX_W - w) / 2, (MAX_H - h) / 2), line, font=font, fill='black')

    im.save(os.path.join(outdir, astr + '.png'))

if len(sys.argv) < 2 or len(sys.argv) > 3:
    print('''Usage: ./scripts/pictogen.py [NUMBER OF PICTOGRAMS=100] [OUTPUT DIR="../pictograms"]''')
    exit(1)
elif len(sys.argv) == 2:
    pictograms = int(sys.argv[1])
else:
    pictograms = int(sys.argv[1])
    output = sys.argv[2]

if not os.path.isdir(output):
    if os.path.exists(output):
        print('''Cannot create directory, file with the same name already exists''')
        exit(1)

    os.makedirs(output, exist_ok=True)

for i in range(pictograms):
    write(i + 1, output)