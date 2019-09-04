import React from 'react'
import { Form as FinalForm, Field } from 'react-final-form'
import { IProfile } from '../../app/models/profile'
import { combineValidators, isRequired } from 'revalidate'
import { Form, Button } from 'semantic-ui-react'
import TextInput from '../../app/common/form/TextInput'
import TextAreaInput from '../../app/common/form/TextAreaInput'
import { observer } from 'mobx-react-lite'

const validate = combineValidators({ displayName: isRequired('displayName') })

interface IProps { profile: IProfile, updateProfile: (profile: IProfile) => void }

const ProfileEditForm: React.FC<IProps> = ({ profile, updateProfile }) => {
  return (
    <FinalForm
      onSubmit={updateProfile} validate={validate} initialValues={profile!}
      render={({ handleSubmit, submitting, invalid, pristine }) => (
        <Form onSubmit={handleSubmit} error>
          <Field name='displayName' placeholder='Display Name' component={TextInput} value={profile!.displayName} />
          <Field name='bio' placeholder='Bio' component={TextAreaInput} value={profile!.bio} />
          <Button positive disabled={invalid || pristine} loading={submitting} content='Update Profile' floated='right' />
        </Form>
      )}
    />
  )
}

export default observer(ProfileEditForm)
