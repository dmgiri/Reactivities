import React, { useContext, useState, FormEvent, useEffect } from 'react'
import { RouteComponentProps } from 'react-router-dom'
import { Segment, Form, Button, Grid } from 'semantic-ui-react'
import { v4 as uuid} from 'uuid'
import { observer } from 'mobx-react-lite'
import ActivityStore from '../../../app/stores/activityStore'


type Event = FormEvent<HTMLInputElement> | FormEvent<HTMLTextAreaElement> 
interface DetailParams { id: string }

const ActivityForm: React.FC<RouteComponentProps<DetailParams>> = ({ match, history }) => {

  const activityStore = useContext(ActivityStore)
  const { createActivity, editActivity, submitting, activity: initialFormState, loadActivity, clearActivity } = activityStore
  const [activity, setActivity] = useState({ id: '', title: '', description: '', category: '', date: '', city: '', venue: '' })
  
  useEffect(() => {
    if (match.params.id && activity.id.length === 0) { loadActivity(match.params.id).then(() => initialFormState && setActivity(initialFormState)) }
    return function cleanup() { clearActivity() }
  }, [loadActivity, match.params.id, activity.id.length, initialFormState, clearActivity])

  const handleChange = (e: Event) => setActivity({ ...activity, [e.currentTarget.name]: e.currentTarget.value })
  const handleSubmit = () => { 
    if (activity.id.length === 0) {
      let newActivity = { ...activity, id: uuid() }; createActivity(newActivity); history.push(`/activities/${newActivity.id}`)
    }
    else { editActivity(activity); history.push(`/activities/${activity.id}`) }
  }

  return (
    <Grid>
      <Grid.Column width={10}>
        <Segment clearing>
          <Form onSubmit={handleSubmit}>
            <Form.Input placeholder='Title' name='title' value={activity.title} onChange={handleChange} />
            <Form.TextArea rows={2} placeholder='Description' name='description' value={activity.description} onChange={handleChange} />
            <Form.Input placeholder='Category' name='category' value={activity.category} onChange={handleChange} />
            <Form.Input type='datetime-local' placeholder='Date' name='date' value={activity.date} onChange={handleChange} />
            <Form.Input placeholder='City' name='city' value={activity.city} onChange={handleChange} />
            <Form.Input placeholder='Venue' name='venue' value={activity.venue} onChange={handleChange} />
            <Button loading={submitting} floated='right' positive type='submit' content='Submit' />
            <Button floated='right' type='button' content='Cancel' onClick={() => history.push('/activities')}/>
          </Form>
        </Segment>
      </Grid.Column>
    </Grid>
  )
}

export default observer(ActivityForm)
