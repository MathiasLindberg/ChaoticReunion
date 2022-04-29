images:
	for i in *.ditaa ; do \
	  echo "$$i -- $${i%.ditaa}.png"; \
	  if [ "$$i" -nt "$${i%.ditaa}.png" ] ; then ditaa -o "$$i"; fi; \
	done

%.png: %.ditaa
	ditaa -o "$<"
