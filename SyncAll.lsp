(defun c:VTSyncAll (/)
  (setq sset(ssget "x" '((0 . "insert"))))
  (setq i 0
	blocksForSync(list))
    (repeat (sslength sset)
      (setq ob(vlax-ename->vla-object (ssname sset i)))
      (setq blocksForSync (cons ob blocksForSync))
      (setq i(1+ i))
      ) 
  (foreach block blocksForSync
    (if(/= (vlax-safearray-get-u-bound(vlax-variant-value(vla-GetAttributes block)) 1)-1)
	(command "attsync" "n" (vla-get-effectiveName block))
   )))