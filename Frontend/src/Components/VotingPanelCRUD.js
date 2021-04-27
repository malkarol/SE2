import React, { useState } from "react";
import { withRouter, Redirect } from "react-router";
import "../Layout/MainLayout.css";

function VotingPanelCRUD(props) {
    const [voting, setVoting] = useState({
        question: "",
        options: [],
        newOption: {
            id: -1,
            optionName: "Fill your new option"
        },
        modifiedOption: {
            id: -1,
            optionName: ""
        }
    });

    const saveVoting = () => {
        alert("Submit voting succesfully");
    }

    const setQuestion = (e) => {
        setVoting({
            ...voting,
            question: e.target.value
        });
    }

    const setNewOption = (e) => {
        setVoting({
            ...voting,
            newOption: {
                ...voting.newOption,
                optionName: e.target.value
            }
        });
    }

    const addOption = (text) => {
        setVoting({
            ...voting,
            options: [
                ...voting.options,
                {
                    ...voting.newOption,
                    id: (new Date()).getTime()
                }
            ],
            newOption: {
                id: -1,
                optionName: "Fill your new option"
            }
        });
    }

    const editOption = (selectedOption, isSaved) => {
        if (isSaved === true) {
            const newOptions = [
                ...voting.options.filter(x => x.id !== voting.modifiedOption.id),
                voting.modifiedOption
            ];
            setVoting({
                ...voting,
                options: newOptions.sort((a, b) => a.id > b.id ? 1 : -1),
                modifiedOption: {
                    id: -1,
                    optionName: ""
                }
            });
        }
        else {
            setVoting({
                ...voting,
                modifiedOption: {
                    id: selectedOption.id,
                    optionName: selectedOption.optionName
                }
            });
        }
    }

    const onChangeOptionName = (text) => {
        setVoting({
            ...voting,
            modifiedOption: {
                ...voting.modifiedOption,
                optionName: text
            }
        });
    }

    const deleteOption = (idOption) => {
        setVoting({
            ...voting,
            options: voting.options.filter(x => x.id !== idOption)
        });
    }

    return (
        <div className="VotingPanel">
            <div className="ChooseText">Question: </div>
            <input className="Question" type="text" onChange={setQuestion} />
            <div className="ChooseText">Answer: </div>
            <div className="Answers">
                {voting.options.map((value, index) => (
                    <div>
                        {value.id === voting.modifiedOption.id
                        ? (<div style={{display: "inline-block"}}>
                            <input 
                                className="AnswerBarCreateWhite"
                                value={voting.modifiedOption.optionName} 
                                onChange= {(e) => onChangeOptionName(e.target.value)}/>
                            <button className="SmallButton" onClick={() => editOption(value, true)}>Save</button>
                        </div>)
                        : (<div style={{display: "inline-block"}}>
                            <input disabled className="AnswerBarCreate" value={value.optionName}/>
                            <button className="SmallButton" onClick={() => editOption(value, false)}>Edit</button>
                        </div>)
                        }
                        <button className="SmallButton" onClick={() => deleteOption(value.id)}>Delete</button>
                    </div>
                ))}
                <div style={{display: "flex"}}>
                    <input 
                        className="AnswerBar" 
                        value={voting.newOption.optionName} 
                        onChange={setNewOption}/>
                    <button className="SmallButton" onClick={addOption}>Add option</button>
                </div>
            </div>
            <div className="AnswerButton">
                <button className="VoteButton" onClick={saveVoting}>Submit</button>
            </div>
        </div>
    )
}


export default (withRouter(VotingPanelCRUD));