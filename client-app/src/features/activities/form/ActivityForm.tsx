import React, { useState, FormEvent } from 'react'
import { Segment, Form, Button } from 'semantic-ui-react'
import { v4 as uuid} from 'uuid'
import { IActivity } from '../../../app/models/activity'

type Event = FormEvent<HTMLInputElement> | FormEvent<HTMLTextAreaElement> 
interface IProps {
  setEditMode: (editMode: boolean) => void, activity: IActivity,
  createActivity: (activity: IActivity) => void, editActivity: (activity: IActivity) => void
}

const ActivityForm: React.FC<IProps> = ({ setEditMode, activity: initialFormState, createActivity, editActivity }) => {

  const initializeForm = () => {
    if (initialFormState) return initialFormState
    else return { id: '', title: '', description: '', category: '', date: '', city: '', venue: '' }
  }

  const [activity, setActivity] = useState(initializeForm)
  const handleChange = (e: Event) => { setActivity({ ...activity, [e.currentTarget.name]: e.currentTarget.value }) }
  const handleSubmit = () => { 
    if (activity.id.length === 0) { let newActivity = { ...activity, id: uuid() }; createActivity(newActivity) }
    else editActivity(activity) // This is just the local shit, we still need to POST/PUT it to the backend
  }

  return (
    <Segment clearing>
      <Form onSubmit={handleSubmit}>
        <Form.Input placeholder='Title' name='title' value={activity.title} onChange={handleChange} />
        <Form.TextArea rows={2} placeholder='Description' name='description' value={activity.description} onChange={handleChange} />
        <Form.Input placeholder='Category' name='category' value={activity.category} onChange={handleChange} />
        <Form.Input type='datetime-local' placeholder='Date' name='date' value={activity.date} onChange={handleChange} />
        <Form.Input placeholder='City' name='city' value={activity.city} onChange={handleChange} />
        <Form.Input placeholder='Venue' name='venue' value={activity.venue} onChange={handleChange} />
        <Button floated='right' positive type='submit' content='Submit' />
        <Button floated='right' type='button' content='Cancel' onClick={() => setEditMode(false)}/>
      </Form>
    </Segment>
  )
}

export default ActivityForm
