#!/usr/bin/env python2.7
import sys
import re

replace_map = { \
  r'external global':'external dllimport global', \
  r'external constant':'external dllimport constant', \
  r'@_TWVBo = external dllimport':'@_TWVBo = external', \
  r'@_TWVBo':'@__imp__TWVBo' \
  }

newf = open(sys.argv[2], "w")
with open(sys.argv[1], "r") as sources:
  lines = sources.readlines()
  for line in lines:
    newline = line
    for k in replace_map.keys():
      newline = re.sub(k, replace_map[k], newline)
#    print newline,
    newf.write(newline)
newf.close()
