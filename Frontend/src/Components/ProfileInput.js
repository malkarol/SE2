import React, { useState } from "react";
import "../Layout/MainLayout.css";

export function InputBox(props){
    let elem = <input id={props.id} onKeyDown={props.noedit} onChange={(e) => props.inputHandler(e, props.pattern)} max={props.maxl}
    min={props.minl} readOnly="true" className="data-inputbox" type={props.inputtype} defaultValue={props.text} />

    if (props.inputtype == "text")
        elem = <input id={props.id} onChange={(e) => props.inputHandler(e, props.pattern)} maxLength="40"
        min={props.minl} readOnly="true" className="data-inputbox" type={props.inputtype} defaultValue={props.text} />
    
    return (
        <div className="input-box">
            <label className="input-desc">{props.inputLabel}</label>
            {elem}
            <label className="error-label" id={"error-" + props.id}>Invalid characters used</label>
        </div>
    )
}
