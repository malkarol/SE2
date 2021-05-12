import React, { useState } from "react";
import { withRouter, Redirect } from "react-router";
import { InputBox } from '../Components/ProfileInput';
import { emailRegex } from '../Components/LoginPage';

function ErrorFound(errorid, elem) {
    const button = document.getElementById("SaveButton");
    document.getElementById(errorid).style.display = "block";
    button.disabled = true;
    button.style.backgroundColor = "#4c5fc09f";
    button.style.cursor = "not-allowed";
    elem.style.border = "2px solid #ed4956";
}
function ErrorSolved(errorid, elem) {
    const button = document.getElementById("SaveButton");
    document.getElementById(errorid).style.display = "none";
    button.disabled = false;
    button.style.backgroundColor = "#4c5fc0";
    button.style.cursor = "pointer";
    elem.style.border = "1px solid #262D34";
}

function verifyInput(e, pattern) {
    const errorid = "error-" + e.target.id;
    const elem = document.getElementById(e.target.id);
    if (e.target.value.length < 1 || !e.target.value.match(pattern)) {
        ErrorFound(errorid, elem);
    } else {
        ErrorSolved(errorid, elem);
    }
}
function verifyTel(e, pattern) {
    const errorid = "error-" + e.target.id;
    const elem = document.getElementById(e.target.id);
    if (e.target.value.length > 8 || !(e.target.value).replace(/\s+/g, '').match(pattern)) {
        ErrorFound(errorid, elem);
    } else {
        ErrorSolved(errorid, elem);
    }
}


function UserPanel(props) {

    const voter = props.voter;
    let isModifying = false;

    function enableButtons() {
        var inputs = document.getElementsByTagName('input');
        if (isModifying) {
            isModifying = false;
            document.getElementById('SaveButton').style.display = "none";
            document.getElementById("ModifyButton").textContent = "Modify";
        }
        else {
            document.getElementById('SaveButton').style.display = "block";
            document.getElementById("ModifyButton").textContent = "Discard changes";
            isModifying = true;
        }
        for (var index = 0; index < inputs.length; ++index) {
            if (!isModifying) {
                inputs[index].readOnly = true;
                inputs[index].value = inputs[index].defaultValue;
                const elem = document.getElementById(inputs[index].id);
                ErrorSolved("error-" + inputs[index].id, elem);
            } else {
                inputs[index].readOnly = false;
            }
        }
    }

    return (
        <div>
            <div className="inputboxes-container">
                    <div className="databoxes-container">
                        <div className="label-row">
                            <h2 className="section-label">Personal information</h2>
                        </div>
                        <div className="data-row">
                            <InputBox id="nameInput" inputHandler={verifyInput} inputtype="text"
                                pattern="^[A-Za-z]+$" inputLabel="Name" text={voter.name} />
                            <InputBox id="surnameInput" inputHandler={verifyInput} inputtype="text"
                                pattern="^[A-Za-z]+$" inputLabel="Surname" text={voter.surname} />
                            <InputBox id="birthInput" noedit={(e) => e.preventDefault()} inputtype="date" minl="1920-01-01"
                                inputHandler={() => { }}
                                maxl={new Date().toISOString().split("T")[0]} inputLabel="Date of birth" text={voter.birth} />
                            <InputBox id="peselInput" inputHandler={verifyInput} inputtype="number"
                                pattern="^\d{11}$" inputLabel="PESEL" text={voter.pesel} />
                        </div>
                    </div>
                    <div className="databoxes-container">
                        <div className="label-row">
                            <h2 className="section-label">Contact information</h2>
                        </div>
                        <div className="data-row">
                            <InputBox id="phoneInput" inputHandler={verifyTel} inputtype="tel"
                                pattern="^\d{9}$"
                                inputLabel="Phone number" text={voter.phone} />
                            <InputBox id="mailInput" inputHandler={verifyInput} inputtype="email"
                                pattern={emailRegex} inputLabel="E-mail" text={voter.email} />
                            <InputBox id="countryInput" inputHandler={verifyInput} inputtype="text"
                                pattern="^[A-Za-z]+$" inputLabel="Country" text={voter.country} />
                            <InputBox id="cityInput" inputHandler={verifyInput} inputtype="text"
                                pattern="^[A-Za-z]+$" inputLabel="City" text={voter.city} />
                            <InputBox id="postInput" inputHandler={verifyInput} inputtype="text"
                                pattern="^([0-9]{2})(-[0-9]{3})?$" inputLabel="Postcode" text={voter.post} />
                            <InputBox id="addressInput" inputHandler={verifyInput} inputtype="text"
                                pattern="^[0-9a-zA-Z\/ -]+$" inputLabel="Address" text={voter.address} />
                        </div>
                    </div>
                </div>
                <div className="button-container">
                    <div className="button-row">
                        <button onClick={enableButtons} id="ModifyButton" className="ModifyButton" type="button">Modify</button>
                        <button onClick={enableButtons} id="SaveButton" className="ModifyButton" type="button">Save</button>
                    </div>
                </div>
        </div>
    )
}


export default (withRouter(UserPanel));