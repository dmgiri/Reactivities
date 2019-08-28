import React, { useContext, useState, FormEvent, useEffect } from 'react'
import { RouteComponentProps } from 'react-router-dom'
import { Segment, Form, Button, Grid } from 'semantic-ui-react'
import { v4 as uuid} from 'uuid'
import { observer } from 'mobx-react-lite'
import { Form as FinalForm, Field } from 'react-final-form'
import ActivityStore from '../../../app/stores/activityStore'
import TextInput from '../../../app/common/form/TextInput'


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

  const handleFinalFormSubmit = (values: any) => { console.log(values) }
  // const handleSubmit = () => { 
  //   if (activity.id.length === 0) {
  //     let newActivity = { ...activity, id: uuid() }; createActivity(newActivity); history.push(`/activities/${newActivity.id}`)
  //   }
  //   else { editActivity(activity); history.push(`/activities/${activity.id}`) }
  // }


  return (
    <Grid>
      <Grid.Column width={10}>
        <Segment clearing>
          <FinalForm onSubmit={handleFinalFormSubmit} render={({ handleSubmit }) => (
            <Form onSubmit={handleSubmit}>
              <Field placeholder='Title' name='title' value={activity.title} component={TextInput} />
              <Field placeholder='Description' name='description' value={activity.description} component={TextInput} />
              <Field placeholder='Category' name='category' value={activity.category} component={TextInput} />
              <Field placeholder='Date' name='date' value={activity.date} component={TextInput} />
              <Field placeholder='City' name='city' value={activity.city} component={TextInput} />
              <Field placeholder='Venue' name='venue' value={activity.venue} component={TextInput} />
              <Button loading={submitting} floated='right' positive type='submit' content='Submit' />
              <Button floated='right' type='button' content='Cancel' onClick={() => history.push('/activities')}/>
            </Form>
          )} />
        </Segment>
      </Grid.Column>
    </Grid>
  )
}

export default observer(ActivityForm)
