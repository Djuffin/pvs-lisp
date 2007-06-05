(pr "Tests are runing...\n")

;TAIL CALL
(defun repeat (n body) 
	(cond ((eq n 1) (body))
		  (t (do (body) (repeat (- n 1) body)  ))))
		  
;(setq count 0)
;(repeat 1000 (lambda () (do (setq count (+ count 1)) (print count) )) )
;(print (length '(1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0) ))

;FACTORIAL TAIL CALL

(defun fact-classic (n) 
	(cond 
		((eq n 0) 1 )
		(t  (* n (fact-classic (- n 1))) )))
		
(defun fact-internal (n m) 
	(cond 
		((eq n 0) m )
		(t  (fact-internal (- n 1) (* m n) ) )))	

(defun fact (n) (fact-internal n 1))
	
(assert (eq (* 1 2 3 4 5 6 7) (fact 7)) "fuck")

;SETQ 
(= foo 5)
(= bar 10)

(assert (and
    (== foo 5)
    (== bar 10)) "SETQ does not work properly")
    
;DEFUN
(= foo (lambda (x y) (+ x y)))
(defun bar (x y) (+ x y) )

(assert (== (foo 123 321) (bar 123 321)) "DEFUN fails 1")
(assert (== (. ToString foo) (. ToString bar)) "DEFUN fails 2")


;DEMACRO
(= foo (macro (x y) (list 'set x y)))
(defmacro bar (x y) (list 'set x y))

(assert (== (. ToString foo) (. ToString bar)) "DEFMACRO fails")

;IF
(assert (if t t nil) "IF fails 1")
(assert (if nil nil t) "IF fails 2")

;EQL
(assert (eql nil nil) "EQL fails 1")
(assert (eql 1 1) "EQL fails 2")
(assert (eql '(a) '(a)) "EQL fails 3")
(assert (eql '(1 (2)) '(1 (2))) "EQL fails 4")
(assert (eql '(1 c (2 (a b))) '(1 c (2 (a b)))) "EQL fails 5")
(assert (eql '((a b c) (c (1 2) b) ((((a)b)c)d) e)   '((a b c) (c (1 2) b) ((((a)b)c)d) e)  ) "EQL fails 5.5")

(assert (not (eql t nil)) "EQL fails 6")
(assert (not (eql '(a b c) '(a b c ()))) "EQL fails 7")
(assert (not (eql '(a c) '(a b c ))) "EQL fails 8")
(assert (not (eql 1 2)) "EQL fails 9")

;CARS
(assert (eql 'a (caaar '(((a b c) a (b c))))) "CAAAR")
(assert (eql '(b c) (caadr '(a ((b c) d)))) "CAADR")
(assert (eql 'a (caar '((a b c) a (b c)))) "CAAR")
(assert (eql 'b (cadar '((a b c) d))) "cadar")
(assert (eql 'e (caddr '((a b c) d e))) "caddr")
(assert (eql 'd (cadr '((a b c) d e))) "cadr")
(assert (eql '(b) (cdaar '(((a b) c) d e))) "cdaar")
(assert (eql '(c) (cdar '(((a b) c) d e))) "cdar")
(assert (eql '(d) (cddar '(((a b) c d) d e))) "cddar")
(assert (eql '(d) (cdddr '(a b c d))) "cdddr")
(assert (eql '(c d) (cddr '(a b c d))) "cddr")
(assert (eql '(b c) (cdr '(a b c))) "cdr")
(assert (eql 'a (car '(a b c))) "car")

;NTH
(assert (== (nth 2 '(a b c d e)) 'c) "NTH fails")

;LENGTH
(assert (== (length '(1 2 3 (1 2 3) 5)) 5) "LENGTH fails") 
(assert (== (length () ) 0) "LENGTH fails (nil)") 
(assert (== (length 1 ) 1) "LENGTH fails (atom)") 

;MAP
(defun inc (x) (+ x 1))
(assert (eql (map inc '(1 2 3 4 5) ) '(2 3 4 5 6)) "MAP fails") 


;LET
(assert 
	(== (let ((x 1) (y 2)) (+ x y)) 3)
	"LET fails")
	
(defun foo (arg) (let () (== arg 123)))	

(assert (foo 123) "LET fails 2")	

;LIST
(assert (eql '(1 (1 2 3) 3 4 a) (list 1 '(1 2 3) 3 4 'a)) "list fails")	


;WIN FORMS
;(.using System.Windows.Forms)
;(.reference System.Windows.Forms)
;(setq frm (.new Form))
;(.call Show frm)
;(.call Text frm "Hello World!")
;(defun handler (sender args) (.call Show MessageBox (.call Text sender) ) )
;(.add-handler Closed frm handler )
;(.add-handler Closed frm '(.call Show MessageBox (.call Text sender) ) )
;(.call Run Application frm)


	


	

