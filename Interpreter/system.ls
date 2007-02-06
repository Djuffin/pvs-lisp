
(print "Hi dude, I'm PVSLisp ;-)\n")

; *********** Basic LISP features *************

(set 'setq 
	(macro (dest source) 
		(list 'set (list 'quote dest) source)))

(setq defun 
	(macro (name params body) 
		(list 'setq name 
			(list 'lambda params body) )))

(setq defmacro
	(macro (name params body) 
		(list 'setq name 
			(list 'macro params body) )))
			
;********** Flow control ********************			
(defmacro if (condition if-statement else-statement)
	(list 'cond (list condition if-statement) 
			    (list 't else-statement) ))			

;*********** List processing ****************
(defun caar (list) (car (car list) ))								
(defun cddr (list) (cdr (cdr list) ))								
(defun cadr (list) (car (cdr list) ))				
(defun cdar (list) (cdr (car list) ))				

(defun cdddr (list) (cdr (cdr (cdr list)) ))
(defun caddr (list) (car (cdr (cdr list)) ))
(defun cdadr (list) (cdr (car (cdr list)) ))
(defun caadr (list) (car (car (cdr list)) ))
(defun cddar (list) (cdr (cdr (car list)) ))
(defun cadar (list) (car (cdr (car list)) ))
(defun cdaar (list) (cdr (car (car list)) ))
(defun caaar (list) (car (car (car list)) ))

(defun eql (list1 list2)
	(cond 
		((and (atom list1) (atom list2)) (eq list1 list2))
		((or (atom list1) (atom list2)) nil)
		(t 
			(if (eql (car list1) (car list2)) 
				(eql (cdr list1) (cdr list2)) 
				nil)  )  )  )
	
(defun nth (index list)
	(if (eq index 0) 
		(car list) 
		(nth (- index 1) (cdr list)) ))				
	

(defun length (list)
	(cond 
		((null list) 0)
		((atom list) 1)
		(t (+ 1 (length (cdr list))))	))				

(defun map (list func) 
	(cond ((null list) nil)
		(t (cons (func (car list)) 
				(map (cdr list) func) ))))
			

; ********* C# style aliases ***********
(setq == eq)
(setq = setq)
(setq true t)
(setq false nil)


; ********* Advanced features *********
(defmacro let (vars body)
	(cons 
		(list 'lambda (map vars car) body)
		(map vars cadr) ))


			
(set 'pr (macro (arg) (list 'print arg)))

;functions