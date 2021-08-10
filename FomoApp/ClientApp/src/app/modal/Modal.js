import './Modal.css';

import React, { useRef } from 'react';
import { useCallback, useEffect } from 'react/cjs/react.development';

import ReactDOM from 'react-dom';

export default function Modal(props) {

    const modalRoot = document.getElementById('modal-root');
    const container = document.createElement('div');
    const thisModalRef = useRef();
    const onCancel = props.onCancel;

    const onCancelCallBack = useCallback( (e) => {
        if (onCancel) {
            onCancel();
        }
    }, [onCancel]);

    useEffect(() => {
        
        function closeWhenClickOutside(e) {
            if (thisModalRef.current.contains(e.target)) {
                return;
            }

            onCancelCallBack();
        }
        
        modalRoot.appendChild(container);
        document.addEventListener("mousedown", closeWhenClickOutside);

        return () => {
            modalRoot.removeChild(container);
            document.removeEventListener("mousedown", closeWhenClickOutside);
        };
    },[container, modalRoot, onCancelCallBack]);

    return (ReactDOM.createPortal(
        <div className='modal-backdrop'>
            <form className='modal-form' ref={thisModalRef}>
                <div className="modal-x-close" onClick={onCancel}>X</div>
                {props.children}
            </form>
        </div>,
        container
    ));
}