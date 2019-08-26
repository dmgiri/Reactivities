import React from 'react'
import { Container } from 'semantic-ui-react'
import { Link } from 'react-router-dom'

const HomePage: React.FC = () => {
  return (
    <Container style={{marginTop: '7em'}}>
      <h1>Home</h1>
      <h3>Go to <Link to='/activities'>Activities</Link></h3>
    </Container>
  )
}

export default HomePage
