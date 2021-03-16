import * as React from 'react'
import styled from 'styled-components'
import { Link, useParams } from 'react-router-dom'
import { GameRouteParams } from './game-route-params'
import { Student, Skill, useStore } from '../store'
import { observer, Observer } from 'mobx-react-lite'
import { Button, Container, makeStyles, TextField, Typography } from '@material-ui/core'

const useStyles = makeStyles((theme) => ({
    paper: {
        marginTop: theme.spacing(8),
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
    },
    paper2: {
        marginTop: theme.spacing(8),
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'flex-start',
    },
    avatar: {
        margin: theme.spacing(1),
        backgroundColor: theme.palette.secondary.main,
    },
    form: {
        width: '100%', // Fix IE 11 issue.
        marginTop: theme.spacing(1),
    },
    submit: {
        margin: theme.spacing(3, 0, 2),
    },
}));

const NewUniversity = observer(() => {
    const { university } = useStore()
    const classes = useStyles()

    const nu = university.newUniversity

    React.useEffect(() => nu.clear(), [])

    return (
        <Container component='main' maxWidth='xs'>
            <div className={classes.paper}>
                <Typography component='h1' variant='h5'>Open new magic university</Typography>
                <TextField
                    variant='outlined'
                    margin='normal'
                    required
                    fullWidth
                    id='name'
                    label='University name'
                    name='name'
                    autoFocus
                    value={nu.name}
                    onChange={nu.setName}
                />
                <Button fullWidth variant='contained' color='primary' className={classes.submit} onClick={nu.open}>Open</Button>
            </div>
        </Container>
    )
})

const StudySchedule = styled.table`
    border-collapse: collapse;

    font-size: 75%;
    font-family: Fira Code, Roboto Mono, monospace;

    td, th {
        padding: 4px;
        white-space: nowrap;
    }

    td {
        border: 1px solid silver;
    }

    th {
        text-align: left;
    }

    .empty {
        border: none;
    }

    .skill {
        padding: 0;

        .level {
            display: inline-block;
            padding: .25rem;
            margin: 2px;
            background-color: silver;
        }

        .days {
            display: inline-block;
            padding: .25rem;
        }
    }

    .faction {
        min-width: 100px;
    }

    .unit {
        min-width: 100px;
    }

    .orders {
        min-width: 50px;
    }

    .target {
        min-width: 50px;
    }

    td.target {
        cursor: pointer;
    }

    .location {
        padding-top: 2rem;
    }

    .skill--can-study {
        background-color: lightgreen;
    }

    .skill--target {
        border-width: 2px;
        border-color: blue;
    }

    .skill--study {
        border-width: 2px;
        border-color: red;
    }

    .skill--tought {
        border-width: 2px;
        border-color: green;
    }
`

function getSkillClasses(student: Student, skill: Skill) {
    const classes = []

    if (student.mode === 'target-selection') {
        if (skill.canStudy) {
            classes.push('skill--can-study')
        }
    }

    if (student.target?.code === skill.code) {
        classes.push('skill--target')
    }

    if (student.study === skill.code) {
        classes.push('skill--study')
    }

    return classes.join(' ')
}

const University = observer(() => {
    const { university } = useStore()
    const classes = useStyles()

    return (
        <Container component='main' maxWidth={false}>
            <div className={classes.paper2}>
                <Typography component='h1' variant='h5'>{university.name}</Typography>
                Turns:
                <ul>
                    { university.classes.map(x => <li key={x.id}>{x.turnNumber}</li> ) }
                </ul>

                <StudySchedule>
                    { university.locations.map(x => <tbody key={x.id}>
                        <tr>
                            <th className='location' colSpan={20}>{x.terrain} ({x.x},{x.y},{x.z} {x.label}) in {x.province}{ x.settlement ? `, contains ${x.settlement} [${x.settlementSize.toLowerCase()}]` : '' }</th>
                        </tr>
                        <tr>
                            <th className='faction'>Faction</th>
                            <th className='unit'>Unit</th>
                            <th className='target'>Target</th>
                            <th className='orders'>Orders</th>

                            { university.skills.map((group, i) => <React.Fragment key={i}>
                                <th className='empty'></th>
                                { group.skills.map(({ code }) => <th key={code}>{code}</th> ) }
                            </React.Fragment> ) }
                        </tr>
                        { x.students.map(student => <tr key={student.id}>
                            <td className='faction'>{student.factionName} ({student.factionNumber})</td>
                            <td className='unit'>{student.name} ({student.number})</td>
                            <td className='target skill' onClick={student.beginTargetSelection}>
                                { student.target && <>
                                        <div className='days'>{student.target.code}</div>
                                        <div className='level' onClick={student.incTargetLevel}>{student.target.level}</div>
                                    </>}
                            </td>
                            <td className='orders'>{student.orders}</td>
                            { student.skillsGroups.map((group, i) => (
                                <React.Fragment key={i}>
                                    <td className='empty'></td>
                                    { group.skills.map(skill => (
                                        <td key={skill.code} className={`skill ${getSkillClasses(student, skill)}`} onClick={() => student.skillClick(skill.code)}>
                                            { skill.days && <>
                                                <div className='days'>{skill.days}</div>
                                                <div className='level'>{skill.level}</div>
                                            </> }
                                        </td>) ) }
                                </React.Fragment>) ) }
                        </tr>)}
                    </tbody> ) }
                </StudySchedule>
            </div>
        </Container>
    )
})

export function UniversityPage() {
    const { gameId } = useParams<GameRouteParams>()
    const { university } = useStore()

    React.useEffect(() => {
        university.load(gameId)
    }, [ gameId ])

    return <Observer>
        {() => university.loading
            ? <div>Loading...</div>
            : university.id
                ? <University />
                : <NewUniversity />
        }
    </Observer>
}

